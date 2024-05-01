using Models.Abstractions;
using Models.Entities;
using Servises.Interfaces;

namespace Servises.Services;

public class TagService : BaseService, ITagService
{
    public TagService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }

    public async Task<bool> AddTagAsync(string name)
    {
        StringEntity? tag = await unitOfWork.TagRepository.GetByNameAsync(name);
        if (tag is null)
        {
            await unitOfWork.TagRepository.AddAsync(new Tag() { Name = name });
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> AddTagToBookAsync(string name, Guid id)
    {
        StringEntity? tag = await unitOfWork.TagRepository.GetByNameWithBooksAsync(name);
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(id);
        if (tag is not null && book is not null)
        {
            tag.Books.Add(book);
            //book.Tags.Add((Tag)tag);
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteTagAsync(string name)
    {
        StringEntity? tag = await unitOfWork.TagRepository.GetByNameAsync(name);
        if (tag is null)
        {
            await unitOfWork.TagRepository.DeleteAsync(name);
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }
    public async Task<IReadOnlyList<string>> AllTagsAsync()
    {
        IReadOnlyList<StringEntity> tags = await unitOfWork.TagRepository.ListAllAsync();
        IReadOnlyList<string> strings = tags.Select(genres => genres.Name).ToList();
        return strings;
    }
}
