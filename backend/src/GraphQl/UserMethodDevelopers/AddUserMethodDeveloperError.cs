using System.Collections.Generic;

namespace Metabase.GraphQl.UserMethodDevelopers;

public sealed class AddUserMethodDeveloperError(
    AddUserMethodDeveloperErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddUserMethodDeveloperErrorCode>(code, message, path)
{
}