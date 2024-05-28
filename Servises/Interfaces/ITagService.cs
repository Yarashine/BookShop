namespace Servises.Interfaces;

public interface ITagService
{
    Task AddTagAsync(string name);
    Task AddTagToBookAsync(string name, Guid id);
    Task DeleteTagAsync(string name);
    Task<IReadOnlyList<string>> AllTagsAsync();
}
