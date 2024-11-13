using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.DataFormats;

[SuppressMessage("Naming", "CA1707")]
public enum CreateDataFormatErrorCode
{
    UNKNOWN,
    TWO_REFERENCES,
    UNKNOWN_MANAGER,
    UNAUTHORIZED
}