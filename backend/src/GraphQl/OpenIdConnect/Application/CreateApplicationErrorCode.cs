using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.OpenIdConnect.Application;

[SuppressMessage("Naming", "CA1707")]
public enum CreateApplicationErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNKNOWN_INSTITUTION
}