using System;
using System.Collections.Generic;
using HotChocolate.Types;

namespace Metabase.GraphQl.DataX;

[InterfaceType("Data")]
public interface IData
{
    Guid Uuid { get; }
    DateTime Timestamp { get; }
    Guid ComponentId { get; }
    string? Name { get; }
    Guid DatabaseId { get; }
    string? Description { get; }
    IReadOnlyList<string> Warnings { get; }
    Guid CreatorId { get; }
    DateTime CreatedAt { get; }
    AppliedMethod AppliedMethod { get; }
    IReadOnlyList<DataApproval> Approvals { get; }
    IReadOnlyList<GetHttpsResource> Resources { get; }
    GetHttpsResourceTree ResourceTree { get; }
    // ResponseApproval Approval { get; }
    string Locale { get; }
}