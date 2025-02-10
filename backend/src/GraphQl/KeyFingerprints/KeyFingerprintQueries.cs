using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Metabase.Data;
using Metabase.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.KeyFingerprints;

[ExtendObjectType(nameof(Query))]
public class KeyFingerprintQueries
{
    public async Task<VerifyKeyFingerprintPayload> VerifyKeyFingerprintAsync(
        KeyFingerprintInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var errors = new List<VerifyKeyFingerprintError>();
        if (!await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
        {
            errors.Add(
                new VerifyKeyFingerprintError(
                    VerifyKeyFingerprintErrorCode.UNKNOWN_INSTITUTION,
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
                new VerifyKeyFingerprintError(
                    VerifyKeyFingerprintErrorCode.UNKNOWN_USER,
                    "Unknown user.",
                    [nameof(input), nameof(input.UserId).FirstCharToLower()]
                )
            );
        }

        if (errors.Count is not 0)
        {
            return new VerifyKeyFingerprintPayload(errors.AsReadOnly());
        }

        var institutionRepresentative = await context.InstitutionRepresentatives
                .FirstOrDefaultAsync(r =>
                    r.InstitutionId == input.InstitutionId
                    && r.UserId == input.UserId
                , cancellationToken).ConfigureAwait(false);

        if (institutionRepresentative == null)
        {
            return new VerifyKeyFingerprintPayload(new VerifyKeyFingerprintError(
                    VerifyKeyFingerprintErrorCode.UNKNOWN_REPRESENTATIVE,
                    "Unknown representative.",
                    [nameof(input), nameof(input.UserId).FirstCharToLower()]
                ));
        }

        if (!institutionRepresentative.KeyFingerprints.Contains(input.KeyFingerprint))
        {
            return new VerifyKeyFingerprintPayload(new VerifyKeyFingerprintError(
                    VerifyKeyFingerprintErrorCode.UNKNOWN_FINGERPRINT,
                    "Unknown keyfingerprint.",
                    [nameof(input), nameof(input.KeyFingerprint).FirstCharToLower()]
                ));
        }

        return new VerifyKeyFingerprintPayload(institutionRepresentative.DataSigningPermission == Enumerations.DataSigningPermission.GRANTED
            || institutionRepresentative.DataSigningPermission == Enumerations.DataSigningPermission.REMOVED);
    }
}