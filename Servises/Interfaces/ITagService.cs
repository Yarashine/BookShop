namespace Servises.Interfaces;

public interface ITagService
{
    Task<bool> AddTagAsync(string name);
    Task<bool> AddTagToBookAsync(string name, Guid id);
    Task<bool> DeleteTagAsync(string name);
    Task<IReadOnlyList<string>> AllTagsAsync();
}
