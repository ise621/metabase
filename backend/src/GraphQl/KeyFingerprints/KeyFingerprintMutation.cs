using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.KeyFingerprints;

[ExtendObjectType(nameof(Mutation))]
public class KeyFingerprintMutation
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddKeyFingerprintPayload> AddKeyFingerprintAsync(
        KeyFingerprintInput input,
        InstitutionByIdDataLoader institutionById,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionRepresentativeAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
        {
            return new AddKeyFingerprintPayload(
                new AddKeyFingerprintError(
                    AddKeyFingerprintErrorCode.UNAUTHORIZED,
                    "You are not authorized to add keyfingerprints.",
                    []
                )
            );
        }

        var errors = new List<AddKeyFingerprintError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
        {
            errors.Add(
                new AddKeyFingerprintError(
                    AddKeyFingerprintErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    [nameof(input), nameof(input.InstitutionId).FirstCharToLower()]
                )
            );
        }

        if (!await context.Users.AsQueryable()
                .Where(u => u.Id == input.UserId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
        {
            errors.Add(
                new AddKeyFingerprintError(
                    AddKeyFingerprintErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    [nameof(input), nameof(input.UserId).FirstCharToLower()]
                )
            );
        }

        if (errors.Count is not 0)
        {
            return new AddKeyFingerprintPayload(errors.AsReadOnly());
        }

        var institutionRepresentative = await context.InstitutionRepresentatives
                .FirstOrDefaultAsync(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                , cancellationToken).ConfigureAwait(false);

        if (institutionRepresentative == null)
        {
            return new AddKeyFingerprintPayload(new AddKeyFingerprintError(
                    AddKeyFingerprintErrorCode.UNKNOWN_REPRESENTATIVE,
                    "Unknown representative.",
                    [nameof(input), nameof(input.UserId).FirstCharToLower()]
                ));
        }

        institutionRepresentative.KeyFingerprints.Add(input.KeyFingerprint);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddKeyFingerprintPayload(input.KeyFingerprint);
    }
}