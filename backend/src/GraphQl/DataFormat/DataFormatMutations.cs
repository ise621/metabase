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
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.DataFormats;

[ExtendObjectType(nameof(Mutation))]
public sealed class DataFormatMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateDataFormatPayload> CreateDataFormatAsync(
        CreateDataFormatInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await DataFormatAuthorization.IsAuthorizedToCreateDataFormatForInstitution(
                claimsPrincipal,
                input.ManagerId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new CreateDataFormatPayload(
                new CreateDataFormatError(
                    CreateDataFormatErrorCode.UNAUTHORIZED,
                    "You are not authorized to create data formats for the institution.",
                    new[] { nameof(input), nameof(input.ManagerId).FirstCharToLower() }
                )
            );

        if (!await context.Institutions.AsQueryable()
                .AnyAsync(
                    x => x.Id == input.ManagerId,
                    cancellationToken
                )
                .ConfigureAwait(false)
           )
            return new CreateDataFormatPayload(
                new CreateDataFormatError(
                    CreateDataFormatErrorCode.UNKNOWN_MANAGER,
                    "Unknown manager.",
                    new[] { nameof(input), nameof(input.ManagerId).FirstCharToLower() }
                )
            );

        if (input.Standard is not null &&
            input.Publication is not null
           )
            return new CreateDataFormatPayload(
                new CreateDataFormatError(
                    CreateDataFormatErrorCode.TWO_REFERENCES,
                    "Specify either a standard or a publication as reference.",
                    new[] { nameof(input), nameof(input.Publication).FirstCharToLower() }
                )
            );

        var dataFormat = new DataFormat(
            input.Name,
            input.Extension,
            input.Description,
            input.MediaType,
            input.SchemaLocator
        )
        {
            ManagerId = input.ManagerId,
            Standard =
                input.Standard is null
                    ? null
                    : new Standard(
                        input.Standard.Title,
                        input.Standard.Abstract,
                        input.Standard.Section,
                        input.Standard.Year,
                        input.Standard.Standardizers,
                        input.Standard.Locator
                    )
                    {
                        Numeration = new Numeration(
                            input.Standard.Numeration.Prefix,
                            input.Standard.Numeration.MainNumber,
                            input.Standard.Numeration.Suffix
                        )
                    },
            Publication =
                input.Publication is null
                    ? null
                    : new Publication(
                        input.Publication.Title,
                        input.Publication.Abstract,
                        input.Publication.Section,
                        input.Publication.Authors,
                        input.Publication.Doi,
                        input.Publication.ArXiv,
                        input.Publication.Urn,
                        input.Publication.WebAddress
                    )
        };
        context.DataFormats.Add(dataFormat);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateDataFormatPayload(dataFormat);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateDataFormatPayload> UpdateDataFormatAsync(
        UpdateDataFormatInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await DataFormatAuthorization.IsAuthorizedToUpdate(
                claimsPrincipal,
                input.DataFormatId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new UpdateDataFormatPayload(
                new UpdateDataFormatError(
                    UpdateDataFormatErrorCode.UNAUTHORIZED,
                    "You are not authorized to the update data format.",
                    new[] { nameof(input) }
                )
            );

        if (input.Standard is not null &&
            input.Publication is not null
           )
            return new UpdateDataFormatPayload(
                new UpdateDataFormatError(
                    UpdateDataFormatErrorCode.TWO_REFERENCES,
                    "Specify either a standard or a publication as reference.",
                    new[] { nameof(input), nameof(input.Publication).FirstCharToLower() }
                )
            );

        var dataFormat =
            await context.DataFormats.AsQueryable()
                .Where(i => i.Id == input.DataFormatId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (dataFormat is null)
            return new UpdateDataFormatPayload(
                new UpdateDataFormatError(
                    UpdateDataFormatErrorCode.UNKNOWN_DATA_FORMAT,
                    "Unknown data format.",
                    new[] { nameof(input), nameof(input.DataFormatId).FirstCharToLower() }
                )
            );

        dataFormat.Update(
            input.Name,
            input.Extension,
            input.Description,
            input.MediaType,
            input.SchemaLocator
        );
        dataFormat.Standard =
            input.Standard is null
                ? null
                : new Standard(
                    input.Standard.Title,
                    input.Standard.Abstract,
                    input.Standard.Section,
                    input.Standard.Year,
                    input.Standard.Standardizers,
                    input.Standard.Locator
                )
                {
                    Numeration = new Numeration(
                        input.Standard.Numeration.Prefix,
                        input.Standard.Numeration.MainNumber,
                        input.Standard.Numeration.Suffix
                    )
                };
        dataFormat.Publication =
            input.Publication is null
                ? null
                : new Publication(
                    input.Publication.Title,
                    input.Publication.Abstract,
                    input.Publication.Section,
                    input.Publication.Authors,
                    input.Publication.Doi,
                    input.Publication.ArXiv,
                    input.Publication.Urn,
                    input.Publication.WebAddress
                );
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateDataFormatPayload(dataFormat);
    }
}