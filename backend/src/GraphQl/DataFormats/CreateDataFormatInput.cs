using System;
using Metabase.GraphQl.References;

namespace Metabase.GraphQl.DataFormats;

public sealed record CreateDataFormatInput(
    string Name,
    string? Extension,
    string Description,
    string MediaType,
    Uri? SchemaLocator,
    ReferenceInput? Reference,
    Guid ManagerId
);