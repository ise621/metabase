using System.Collections.Generic;
using Enum = System.Enum;

namespace Metabase.GraphQl;

public abstract class UserErrorBase<TUserErrorCode>(
    TUserErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
    : IUserError
    where TUserErrorCode : struct, Enum
{
    public TUserErrorCode Code { get; } = code;
    public string Message { get; } = message;
    public IReadOnlyList<string> Path { get; } = path;
}