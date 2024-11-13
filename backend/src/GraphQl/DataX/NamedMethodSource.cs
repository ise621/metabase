namespace Metabase.GraphQl.DataX;

public sealed class NamedMethodSource(
    string name,
    CrossDatabaseDataReference value
    )
{
    public string Name { get; } = name;
    public CrossDatabaseDataReference Value { get; } = value;
}