using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.DataFormats;

namespace Metabase.GraphQl.DataX;

public sealed class FileMetaInformation(
    IReadOnlyList<string> path,
    Guid dataFormatId
    )
{
    public IReadOnlyList<string> Path { get; } = path;
    public Guid DataFormatId { get; } = dataFormatId;

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