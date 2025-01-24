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
using Metabase.GraphQl.Common;
using Metabase.GraphQl.Publications;
using Metabase.GraphQl.Standards;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.Methods;

[ExtendObjectType(nameof(Mutation))]
public sealed class MethodMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateMethodPayload> CreateMethodAsync(
        CreateMethodInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await MethodAuthorization.IsAuthorizedToCreateMethodManagedByInstitution(
                claimsPrincipal,
                input.ManagerId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
        {
            return new CreateMethodPayload(
                new CreateMethodError(
                    CreateMethodErrorCode.UNAUTHORIZED,
                    "You are not authorized to create methods for the institution.",
                    [nameof(input), nameof(input.ManagerId).FirstCharToLower()]
                )
            );
        }

        if (!await context.Institutions.AsQueryable()
                .AnyAsync(
                    x => x.Id == input.ManagerId,
                    cancellationToken
                )
                .ConfigureAwait(false)
           )
        {
            return new CreateMethodPayload(
                new CreateMethodError(
                    CreateMethodErrorCode.UNKNOWN_MANAGER,
                    "Unknown manager.",
                    [nameof(input), nameof(input.ManagerId).FirstCharToLower()]
                )
            );
        }

        var unknownInstitutionDeveloperIds =
            input.InstitutionDeveloperIds.Except(
                await context.Institutions.AsQueryable()
                    .Where(x => input.InstitutionDeveloperIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false)
            );
        if (unknownInstitutionDeveloperIds.Any())
        {
            return new CreateMethodPayload(
                new CreateMethodError(
                    CreateMethodErrorCode.UNKNOWN_INSTITUTION_DEVELOPERS,
                    $"There are no institutions with identifier(s) {string.Join(", ", unknownInstitutionDeveloperIds)}.",
                    [nameof(input), nameof(input.InstitutionDeveloperIds).FirstCharToLower()]
                )
            );
        }

        var unknownUserDeveloperIds =
            input.UserDeveloperIds.Except(
                await context.Users.AsQueryable()
                    .Where(u => input.UserDeveloperIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false)
            );
        if (unknownUserDeveloperIds.Any())
        {
            return new CreateMethodPayload(
                new CreateMethodError(
                    CreateMethodErrorCode.UNKNOWN_USER_DEVELOPERS,
                    $"There are no users with identifier(s) {string.Join(", ", unknownUserDeveloperIds)}.",
                    [nameof(input), nameof(input.UserDeveloperIds).FirstCharToLower()]
                )
            );
        }

        if (input.Reference?.Standard is not null &&
            input.Reference?.Publication is not null
           )
        {
            return new CreateMethodPayload(
                new CreateMethodError(
                    CreateMethodErrorCode.TWO_REFERENCES,
                    "Specify either a standard or a publication as reference.",
                    [nameof(input), nameof(input.Reference).FirstCharToLower()]
                )
            );
        }

        var method = new Method(
            input.Name,
            input.Description,
            input.Validity is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Validity),
            input.Availability is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Availability),
            input.CalculationLocator,
            input.Categories
        )
        {
            ManagerId = input.ManagerId,
            Standard =
                input.Reference?.Standard is null
                    ? null
                    : StandardType.FromInput(input.Reference.Standard),
            Publication =
                input.Reference?.Publication is null
                    ? null
                    : PublicationType.FromInput(input.Reference.Publication),
        };
        foreach (var institutionDeveloperId in input.InstitutionDeveloperIds.Distinct())
        {
            method.InstitutionDeveloperEdges.Add(
                new InstitutionMethodDeveloper
                {
                    InstitutionId = institutionDeveloperId,
                    Pending = !await InstitutionMethodDeveloperAuthorization.IsAuthorizedToConfirm(claimsPrincipal,
                        institutionDeveloperId, userManager, context, cancellationToken).ConfigureAwait(false)
                }
            );
        }

        var loggedInUser = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        foreach (var userDeveloperId in input.UserDeveloperIds.Distinct())
        {
            method.UserDeveloperEdges.Add(
                new UserMethodDeveloper
                {
                    UserId = userDeveloperId,
                    Pending = !await UserMethodDeveloperAuthorization
                        .IsAuthorizedToConfirm(claimsPrincipal, userDeveloperId, userManager).ConfigureAwait(false)
                }
            );
        }

        context.Methods.Add(method);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateMethodPayload(method);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateMethodPayload> UpdateMethodAsync(
        UpdateMethodInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await MethodAuthorization.IsAuthorizedToUpdate(
                claimsPrincipal,
                input.MethodId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
        {
            return new UpdateMethodPayload(
                new UpdateMethodError(
                    UpdateMethodErrorCode.UNAUTHORIZED,
                    "You are not authorized to the update method.",
                    [nameof(input)]
                )
            );
        }

        if (input.Reference?.Standard is not null &&
            input.Reference?.Publication is not null
           )
        {
            return new UpdateMethodPayload(
                new UpdateMethodError(
                    UpdateMethodErrorCode.TWO_REFERENCES,
                    "Specify either a standard or a publication as reference.",
                    [nameof(input), nameof(input.Reference).FirstCharToLower()]
                )
            );
        }

        var method =
            await context.Methods.AsQueryable()
                .Where(i => i.Id == input.MethodId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (method is null)
        {
            return new UpdateMethodPayload(
                new UpdateMethodError(
                    UpdateMethodErrorCode.UNKNOWN_METHOD,
                    "Unknown method.",
                    [nameof(input), nameof(input.MethodId).FirstCharToLower()]
                )
            );
        }

        method.Update(
            input.Name,
            input.Description,
            input.Validity is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Validity),
            input.Availability is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Availability),
            input.CalculationLocator,
            input.Categories
        );
        method.Standard =
            input.Reference?.Standard is null
                ? null
                : StandardType.FromInput(input.Reference.Standard);
        method.Publication =
            input.Reference?.Publication is null
                ? null
                : PublicationType.FromInput(input.Reference.Publication);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateMethodPayload(method);
    }
}