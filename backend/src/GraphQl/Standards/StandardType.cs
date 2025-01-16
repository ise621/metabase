using System;
using HotChocolate.Types;
using Metabase.Data;

namespace Metabase.GraphQl.Standards;

public sealed class StandardType
    : ObjectType<Standard>
{
    internal static Standard FromInput(StandardInput input)
    {
        return new Standard(
            input.Title,
            input.Abstract,
            input.Section,
            input.Year,
            input.Standardizers,
            input.Locator
        )
        {
            Numeration = new Numeration(
                input.Numeration.Prefix,
                input.Numeration.MainNumber,
                input.Numeration.Suffix
            )
        };
    }

    protected override void Configure(
        IObjectTypeDescriptor<Standard> descriptor
    )
    {
    }
}