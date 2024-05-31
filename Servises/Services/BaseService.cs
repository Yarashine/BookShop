using Models.Abstractions;

namespace Servises.Services;

public abstract class BaseService(IUnitOfWork _unitOfWork)
{
    protected readonly IUnitOfWork unitOfWork = _unitOfWork;
}
