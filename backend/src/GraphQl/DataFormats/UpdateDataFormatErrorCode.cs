using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.DataFormats;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateDataFormatErrorCode
{
    UNKNOWN,
    TWO_REFERENCES,
    UNAUTHORIZED,
    UNKNOWN_DATA_FORMAT
}