using Microsoft.AspNetCore.Authorization;
using Models.Entities;

namespace Api.AuthorizationPolicy.IsBlocked;

public class IsBlockedRequirement : IAuthorizationRequirement
{
    public StateType UserType { get; } = StateType.IsBlocked;

    public IsBlockedRequirement()
    {

    }
}

