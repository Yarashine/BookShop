using Microsoft.AspNetCore.Authorization;
using Models.Entities;

namespace Api.AuthorizationPolicy.IsBunned;

public class IsBunnedRequirement : IAuthorizationRequirement
{
    public StateType UserType { get; } = StateType.IsBlocked;

    public IsBunnedRequirement()
    {

    }
}

