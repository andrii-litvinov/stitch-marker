using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SM.Service.Messages;

namespace SM.Service.AuthorizationHandlers
{
    public class PatternAuthorizationHandler : AuthorizationHandler<OwnerRequirement, Pattern>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OwnerRequirement requirement,
            Pattern resource)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userId == null) return Task.CompletedTask;

            if (userId.Value == resource.OwnerId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class OwnerRequirement : IAuthorizationRequirement
    {
    }
}
