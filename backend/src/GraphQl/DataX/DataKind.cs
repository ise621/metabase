using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.DataX;

[SuppressMessage("Naming", "CA1707")]
public enum DataKind
{
    CALORIMETRIC_DATA,
    HYGROTHERMAL_DATA,
    OPTICAL_DATA,
    PHOTOVOLTAIC_DATA,
    GEOMETRIC_DATA
}