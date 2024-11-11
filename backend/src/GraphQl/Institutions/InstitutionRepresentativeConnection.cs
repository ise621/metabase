using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionRepresentativeConnection
    : ForkingConnection<Institution, InstitutionRepresentative,
        PendingInstitutionRepresentativesByInstitutionIdDataLoader,
        InstitutionRepresentativesByInstitutionIdDataLoader, InstitutionRepresentativeEdge>
{
    public InstitutionRepresentativeConnection(
        Institution institution,
        bool pending
    )
        : base(
            institution,
            pending,
            x => new InstitutionRepresentativeEdge(x)
        )
    {
    }

    [UseUserManager]
    public Task<bool> CanCurrentUserAddEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return InstitutionRepresentativeAuthorization.IsAuthorizedToManage(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}