using System;
using HotChocolate.Types;
using Metabase.Data;
using Metabase.GraphQl.Publications;
using Metabase.GraphQl.References;
using Metabase.GraphQl.Standards;

namespace Metabase.GraphQl.DescriptionOrReferences;

public sealed class DescriptionOrReferenceType
    : ObjectType<DescriptionOrReference>
{
    internal static DescriptionOrReference FromInput(DescriptionOrReferenceInput input)
    {
        if (input.Reference?.Standard is not null && input.Reference?.Publication is not null)
            throw new ArgumentException("Both the reference's standard and publication are non-null.", nameof(input));
        Reference? reference = null;
        if (input.Reference?.Standard is not null)
            reference = new Reference(StandardType.FromInput(input.Reference.Standard));
        else if (input.Reference?.Publication is not null)
            reference = new Reference(PublicationType.FromInput(input.Reference.Publication));
        if (reference is null && input.Description is not null)
            return new DescriptionOrReference(input.Description);
        else if (reference is not null && input?.Description is null)
            return new DescriptionOrReference(reference);
        else if (reference is not null && input?.Description is not null)
            return new DescriptionOrReference(reference, input.Description);
        throw new ArgumentException("Impossible!", nameof(input));
    }

    protected override void Configure(
        IObjectTypeDescriptor<DescriptionOrReference> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(t => t.Exists)
            .Ignore();
        descriptor
            .Field(t => t.Reference)
            .Type<ReferenceType>()
            .Resolve(context => context
                .Parent<DescriptionOrReference>()
                .Reference?
                .TheReference
            );
    }
}