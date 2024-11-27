using HotChocolate.Types;
using NpgsqlTypes;
using DateTime = System.DateTime;

namespace Metabase.GraphQl.Common;

public sealed class OpenEndedDateTimeRangeType
    : ObjectType<NpgsqlRange<DateTime>>
{
    public static NpgsqlRange<DateTime> FromInput(
        OpenEndedDateTimeRangeInput input
    )
    {
        return new NpgsqlRange<DateTime>(
            input.From.GetValueOrDefault(), true, input.From is null,
            input.To.GetValueOrDefault(), true, input.To is null
        );
    }

    protected override void Configure(
        IObjectTypeDescriptor<NpgsqlRange<DateTime>> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();

        const string SuffixedName = nameof(OpenEndedDateTimeRangeType);
        descriptor.Name(SuffixedName.Remove(SuffixedName.Length - "Type".Length));

        descriptor
            .Field("from")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<DateTime>>();
                    return range.LowerBoundInfinite
                        ? null
                        : range.LowerBound;
                }
            );

        descriptor
            .Field("to")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<DateTime>>();
                    return range.UpperBoundInfinite
                        ? null
                        : range.UpperBound;
                }
            );
    }
}