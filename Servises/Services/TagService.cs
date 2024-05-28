using Models.Abstractions;
using Models.Entities;
using Servises.Interfaces;
using Models.Exceptions;

namespace Servises.Services;

public class TagService : BaseService, ITagService
{
    public TagService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }

    public async Task AddTagAsync(string name)
    {
        Tag? tag = await unitOfWork.TagRepository.GetByNameAsync(name);
        if (tag is not null)
            throw new BadRequestException("This tag is already exist");
        await unitOfWork.TagRepository.AddAsync(new Tag() { Name = name });
        await unitOfWork.SaveAllAsync();
    }

    public async Task AddTagToBookAsync(string name, Guid id)
    {
        Tag tag = await unitOfWork.TagRepository.GetByNameWithBooksAsync(name)
            ?? throw new NotFoundException(nameof(Tag));
        Book book = await unitOfWork.BookRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Book));
        tag.Books.Add(book);
        await unitOfWork.SaveAllAsync();
    }

    public async Task DeleteTagAsync(string name)
    {
        Tag tag = await unitOfWork.TagRepository.GetByNameAsync(name)
            ?? throw new NotFoundException(nameof(Tag));
        await unitOfWork.TagRepository.DeleteAsync(name);
        await unitOfWork.SaveAllAsync();
    }
    public async Task<IReadOnlyList<string>> AllTagsAsync()
    {
        IReadOnlyList<Tag> tags = await unitOfWork.TagRepository.ListAllAsync();
        IReadOnlyList<string> strings = tags.Select(genres => genres.Name).ToList();
        return strings;
    }
}
