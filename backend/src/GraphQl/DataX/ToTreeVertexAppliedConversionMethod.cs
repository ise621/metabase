using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.DataX;

public sealed class ToTreeVertexAppliedConversionMethod(
    Guid methodId,
    IReadOnlyList<NamedMethodArgument> arguments,
    string sourceName
    )
{
    public Guid MethodId { get; } = methodId;
    public IReadOnlyList<NamedMethodArgument> Arguments { get; } = arguments;
    public string SourceName { get; } = sourceName;

    public Task<Method?> GetMethodAsync(
        MethodByIdDataLoader methodById,
        CancellationToken cancellationToken
    )
    {
        return methodById.LoadAsync(
            MethodId,
            cancellationToken
        );
    }
}