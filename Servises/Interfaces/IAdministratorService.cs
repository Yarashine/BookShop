using Models.Dtos;
using Models.Entities;
namespace Servises.Interfaces;

public interface IAdministratorService
{
    Task<List<UnbanRequestDto>> AllUnblockRequestsAsync();
    Task BlockUserAsync(Guid AdministratorId, BlockedStatusDto status);
    Task BlockBookAsync(Guid AdministratorId, BlockedStatusDto status);
    Task BlockCommentAsync(Guid AdministratorId, BlockedStatusDto status);
    Task UnBlockUserAsync(Guid id);
    Task UnBlockBookAsync(Guid id);
    Task UnBlockCommentAsync(Guid id);
}
