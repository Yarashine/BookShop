using Models.Abstractions;

namespace Servises.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork unitOfWork;
    public BaseService(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }
}
