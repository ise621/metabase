using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class RemoveComponentAssemblyError(
    RemoveComponentAssemblyErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveComponentAssemblyErrorCode>(code, message, path)
{
}