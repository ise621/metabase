using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.DataX;

[SuppressMessage("Naming", "CA1707")]
public enum OpticalComponentSubtype
{
    MONOLITHIC,
    LAMINATE,
    INTERLAYER,
    EMBEDDED_COATING,
    COATED,
    COATING,
    APPLIED_FILM,
    FILM,
    VENETIAN_BLIND,
    DIFFUSING_SHADE,
    ROLLER_SHADE,
    WOVEN_SHADE,
    VERTICAL_LOUVER,
    PERFORATED_SCREEN,
    CELLULAR_SHADE,
    PLEATED_SHADE,
    ROMAN_SHADE,
    SHADE_MATERIAL,
    FRITTED_GLASS,
    ACID_ETCHED_GLASS,
    SANDBLASTED_GLASS,
    CHROMOGENIC
}