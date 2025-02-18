using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.DataFormats;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResource(
    string? description,
    string hashValue,
    Uri locator,
    Guid dataFormatId,
    IReadOnlyList<FileMetaInformation> archivedFilesMetaInformation
    )
{
    public string? Description { get; } = description;
    public string HashValue { get; } = hashValue;
    public Uri Locator { get; } = locator;
    public Guid DataFormatId { get; } = dataFormatId;
    public IReadOnlyList<FileMetaInformation> ArchivedFilesMetaInformation { get; } = archivedFilesMetaInformation;

    public Task<DataFormat?> GetDataFormatAsync(
        DataFormatByIdDataLoader dataFormatById,
        CancellationToken cancellationToken
    )
    {
        return dataFormatById.LoadAsync(
            DataFormatId,
            cancellationToken
        );
    }
}