using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.DataFormats;

public sealed class DataFormatSortType
    : SortInputType<DataFormat>
{
    protected override void Configure(
        ISortInputTypeDescriptor<DataFormat> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Extension);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.MediaType);
        descriptor.Field(x => x.SchemaLocator);
        descriptor.Field(x => x.Manager);
    }
}