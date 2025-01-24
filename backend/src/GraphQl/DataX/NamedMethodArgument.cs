using System.Text.Json;

namespace Metabase.GraphQl.DataX;

public sealed class NamedMethodArgument(
    string name,
    JsonElement value
    )
{
    public string Name { get; } = name;
    public JsonElement Value { get; } = value;
}