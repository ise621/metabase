using Metabase.Data;

namespace Metabase.GraphQl.DataFormats;

public sealed class CreateDataFormatPayload
    : DataFormatPayload<CreateDataFormatError>
{
    public CreateDataFormatPayload(
        DataFormat standard
    )
        : base(standard)
    {
    }

    public CreateDataFormatPayload(
        CreateDataFormatError error
    )
        : base(error)
    {
    }
}