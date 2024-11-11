using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManagedDataFormatConnection
    : Connection<Institution, DataFormat, InstitutionManagedDataFormatsByInstitutionIdDataLoader,
        InstitutionManagedDataFormatEdge>
{
    public InstitutionManagedDataFormatConnection(
        Institution institution
    )
        : base(
            institution,
            x => new InstitutionManagedDataFormatEdge(x)
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
        return DataFormatAuthorization.IsAuthorizedToCreateDataFormatForInstitution(
            claimsPrincipal,
            Subject.Id,
            userManager,
            context,
            cancellationToken
        );
    }
}