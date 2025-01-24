using System.Collections.Generic;

namespace Metabase.GraphQl.Users;

public sealed class SetUserPhoneNumberError(
    SetUserPhoneNumberErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<SetUserPhoneNumberErrorCode>(code, message, path)
{
}