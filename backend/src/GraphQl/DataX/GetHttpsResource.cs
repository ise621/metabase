using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.DataFormats;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResource(
    string description,
    string hashValue,
    Uri locator,
    Guid dataFormatId,
    IReadOnlyList<FileMetaInformation> archivedFilesMetaInformation
    )
{
    private const string BedJsonGuid = "9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708";
    private const string LbnlKlemsGuid = "e021cf20-e887-4dce-ad27-35da70cec472";

    private static Guid GuessDataFormatId(Uri locator)
    {
        if (locator.Query.Contains("bed-json"))
        {
            return new Guid(BedJsonGuid);
        }
        return new Guid(LbnlKlemsGuid);
    }

    public string Description { get; } = description;
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