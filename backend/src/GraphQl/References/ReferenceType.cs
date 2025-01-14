using System;
using HotChocolate.Types;
using Metabase.Data;

namespace Metabase.GraphQl.References;

public sealed class ReferenceType
    : InterfaceType<IReference>
{
    protected override void Configure(IInterfaceTypeDescriptor<IReference> descriptor)
    {
        descriptor.Name(nameof(IReference)[1..]);
    }
}