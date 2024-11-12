using System;
using Metabase.Enumerations;
using Metabase.GraphQl.Common;
using Metabase.GraphQl.References;

namespace Metabase.GraphQl.Methods;

public sealed record CreateMethodInput(
    string Name,
    string Description,
    OpenEndedDateTimeRangeInput? Validity,
    OpenEndedDateTimeRangeInput? Availability,
    ReferenceInput? Reference,
    Uri? CalculationLocator,
    MethodCategory[] Categories,
    Guid ManagerId,
    Guid[] InstitutionDeveloperIds,
    Guid[] UserDeveloperIds
);