using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class AddComponentAssemblyError(
    AddComponentAssemblyErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddComponentAssemblyErrorCode>(code, message, path)
{
}