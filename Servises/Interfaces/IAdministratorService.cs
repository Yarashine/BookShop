using Models.Dtos;
using Models.Entities;
namespace Servises.Interfaces;

public interface IAdministratorService
{
    Task<List<UnbanRequestDto>> AllUnblockRequestsAsync();
    Task BlockUserAsync(BlockedStatusDto status);
    Task BlockBookAsync(BlockedStatusDto status);
    Task BlockCommentAsync(BlockedStatusDto status);
    Task UnBlockUserAsync(Guid id);
    Task UnBlockBookAsync(Guid id);
    Task UnBlockCommentAsync(Guid id);
}
