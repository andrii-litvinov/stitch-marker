using Microsoft.AspNetCore.Authorization;
using SM.Service.Messages;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Service.AuthorizationHandlers
{
    public class PatternAuthorizationHandler : AuthorizationHandler<AuthorRequirement, Pattern>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorRequirement requirement,
                                                       Pattern resource)
        {
            if (context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class AuthorRequirement : IAuthorizationRequirement { }
}
