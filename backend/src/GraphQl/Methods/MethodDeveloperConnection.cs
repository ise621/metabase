using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Methods;

public sealed class MethodDeveloperConnection(
    Method subject,
    bool pending
    )
{
    private readonly bool _pending = pending;
    private readonly Method _subject = subject;

    public async Task<IEnumerable<MethodDeveloperEdge>> GetEdgesAsync(
        InstitutionMethodDevelopersByMethodIdDataLoader institutionMethodDevelopersDataLoader,
        UserMethodDevelopersByMethodIdDataLoader userMethodDevelopersDataLoader,
        PendingInstitutionMethodDevelopersByMethodIdDataLoader pendingInstitutionMethodDevelopersDataLoader,
        PendingUserMethodDevelopersByMethodIdDataLoader pendingUserMethodDevelopersDataLoader,
        CancellationToken cancellationToken
    )
    {
        return
            (await new InstitutionMethodDeveloperConnection(_subject, _pending)
                .GetEdgesAsync(
                    pendingInstitutionMethodDevelopersDataLoader,
                    institutionMethodDevelopersDataLoader,
                    cancellationToken
                )
                .ConfigureAwait(false)
            )
            .Select(e => new MethodDeveloperEdge(e))
            .Concat(
                (await new UserMethodDeveloperConnection(_subject, _pending)
                    .GetEdgesAsync(
                        pendingUserMethodDevelopersDataLoader,
                        userMethodDevelopersDataLoader,
                        cancellationToken
                    )
                    .ConfigureAwait(false)
                )
                .Select(e => new MethodDeveloperEdge(e))
            );
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserAddInstitutionEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return InstitutionMethodDeveloperAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            _subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserAddUserEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return UserMethodDeveloperAuthorization.IsAuthorizedToAdd(
            claimsPrincipal,
            _subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}

internal sealed class InstitutionMethodDeveloperConnection(
    Method subject,
    bool pending
    )
        : ForkingConnection<Method, InstitutionMethodDeveloper,
        PendingInstitutionMethodDevelopersByMethodIdDataLoader, InstitutionMethodDevelopersByMethodIdDataLoader,
        InstitutionMethodDeveloperEdge>(
        subject,
        pending,
        x => new InstitutionMethodDeveloperEdge(x)
        )
{
}

internal sealed class UserMethodDeveloperConnection(
    Method subject,
    bool pending
    )
        : ForkingConnection<Method, UserMethodDeveloper, PendingUserMethodDevelopersByMethodIdDataLoader,
        UserMethodDevelopersByMethodIdDataLoader, UserMethodDeveloperEdge>(
        subject,
        pending,
        x => new UserMethodDeveloperEdge(x)
        )
{
}