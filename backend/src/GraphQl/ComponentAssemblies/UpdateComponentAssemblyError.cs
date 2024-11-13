using System.Collections.Generic;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class UpdateComponentAssemblyError(
    UpdateComponentAssemblyErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateComponentAssemblyErrorCode>(code, message, path)
{
}