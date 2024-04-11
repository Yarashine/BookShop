using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Models.Entities;
using Models.Abstractions;


namespace Api.AuthorizationPolicy.IsBunned;

public class IsBunnedHandler : AuthorizationHandler<IsBunnedRequirement>
{
    private IUnitOfWork _unitOfWork;

    public IsBunnedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<StateType?> StateTypeById(Guid id)
    {
        User? user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user is null) return null;
        return user.State;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsBunnedRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
        {
            return;
        }
        Claim? id = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        Claim? role = context.User.FindFirst(c => c.Type == ClaimTypes.Role);
        if (role is not null && role.Value.ToString() == "Admin")
        {
            context.Succeed(requirement);
            return;
        }
        if (id is not null)
        {
            StateType? stayType = await StateTypeById(Guid.Parse(id.Value.ToString()));

            if (stayType is not null && stayType != StateType.IsBanned)
            {
                context.Succeed(requirement);
            }
        }
    }
}