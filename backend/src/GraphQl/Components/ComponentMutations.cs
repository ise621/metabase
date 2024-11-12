using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
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

namespace Metabase.GraphQl.Components;

[ExtendObjectType(nameof(Mutation))]
public sealed class ComponentMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateComponentPayload> CreateComponentAsync(
        CreateComponentInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentAuthorization.IsAuthorizedToCreateComponentForInstitution(
                claimsPrincipal,
                input.ManufacturerId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new CreateComponentPayload(
                new CreateComponentError(
                    CreateComponentErrorCode.UNAUTHORIZED,
                    "You are not authorized to create components for the institution.",
                    [nameof(input), nameof(input.ManufacturerId).FirstCharToLower()]
                )
            );

        if (!await context.Institutions.AsQueryable()
                .AnyAsync(
                    x => x.Id == input.ManufacturerId,
                    cancellationToken
                )
                .ConfigureAwait(false)
           )
            return new CreateComponentPayload(
                new CreateComponentError(
                    CreateComponentErrorCode.UNKNOWN_MANUFACTURER,
                    "Unknown manufacturer",
                    [nameof(input), nameof(input.ManufacturerId).FirstCharToLower()]
                )
            );

        if (input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Standard is not null
            && input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Publication is not null)
        {
            return new CreateComponentPayload(
                new CreateComponentError(
                    CreateComponentErrorCode.AMBIGUOUS_REFERENCE_IN_DEFINITION_OF_SURFACES_AND_PRIME_DIRECTION,
                    "Both standard and publication are non-null.",
                    [nameof(input), nameof(input.DefinitionOfSurfacesAndPrimeDirection).FirstCharToLower(), nameof(input.DefinitionOfSurfacesAndPrimeDirection.Reference).FirstCharToLower()]
                )
            );
        }

        var component = new Component(
            input.Name,
            input.Abbreviation,
            input.Description,
            input.Availability is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Availability),
            input.Categories
        );
        // Note that above we make sure that standard and publication are *not* both non-null.
        Reference? reference = null;
        if (input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Standard is not null)
            reference = new Reference(StandardType.FromInput(input.DefinitionOfSurfacesAndPrimeDirection.Reference.Standard));
        else if (input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Publication is not null)
            reference = new Reference(PublicationType.FromInput(input.DefinitionOfSurfacesAndPrimeDirection.Reference.Publication));
        if (reference is null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is not null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(input.DefinitionOfSurfacesAndPrimeDirection.Description);
        else if (reference is not null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(reference);
        else if (reference is not null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is not null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(reference, input.DefinitionOfSurfacesAndPrimeDirection.Description);

        component.ManufacturerEdges.Add(
                        new ComponentManufacturer
                        {
                            InstitutionId = input.ManufacturerId,
                            Pending = false
                        }
                    );
        context.Components.Add(component);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateComponentPayload(component);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateComponentPayload> UpdateComponentAsync(
        UpdateComponentInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentAuthorization.IsAuthorizedToUpdate(
                claimsPrincipal,
                input.ComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new UpdateComponentPayload(
                new UpdateComponentError(
                    UpdateComponentErrorCode.UNAUTHORIZED,
                    "You are not authorized to update the component.",
                    []
                )
            );

        var component =
            await context.Components.AsQueryable()
                .Where(i => i.Id == input.ComponentId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (component is null)
            return new UpdateComponentPayload(
                new UpdateComponentError(
                    UpdateComponentErrorCode.UNKNOWN_COMPONENT,
                    "Unknown component.",
                    [nameof(input), nameof(input.ComponentId).FirstCharToLower()]
                )
            );

        component.Update(
            input.Name,
            input.Abbreviation,
            input.Description,
            input.Availability is null
                ? null
                : OpenEndedDateTimeRangeType.FromInput(input.Availability),
            input.Categories
        );
        // Note that above we make sure that standard and publication are *not* both non-null.
        Reference? reference = null;
        if (input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Standard is not null)
            reference = new Reference(StandardType.FromInput(input.DefinitionOfSurfacesAndPrimeDirection.Reference.Standard));
        else if (input.DefinitionOfSurfacesAndPrimeDirection?.Reference?.Publication is not null)
            reference = new Reference(PublicationType.FromInput(input.DefinitionOfSurfacesAndPrimeDirection.Reference.Publication));
        if (reference is null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is not null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(input.DefinitionOfSurfacesAndPrimeDirection.Description);
        else if (reference is not null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(reference);
        else if (reference is not null && input.DefinitionOfSurfacesAndPrimeDirection?.Description is not null)
            component.DefinitionOfSurfacesAndPrimeDirection = new DefinitionOfSurfacesAndPrimeDirection(reference, input.DefinitionOfSurfacesAndPrimeDirection.Description);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateComponentPayload(component);
    }
}