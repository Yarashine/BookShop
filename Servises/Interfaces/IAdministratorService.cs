using Models.Dtos;
using Models.Entities;
namespace Servises.Interfaces;

public interface IAdministratorService
{
    Task<bool> BlockUserAsync(BlockedStatusDto status);
    Task<bool> BlockBookAsync(BlockedStatusDto status);
    Task<bool> BlockCommentAsync(BlockedStatusDto status);
    Task<bool> UnBlockUserAsync(Guid id);
    Task<bool> UnBlockBookAsync(Guid id);
    Task<bool> UnBlockCommentAsync(Guid id);
}
