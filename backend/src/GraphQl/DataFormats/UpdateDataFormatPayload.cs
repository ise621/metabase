using Metabase.Data;

namespace Metabase.GraphQl.DataFormats;

public sealed class UpdateDataFormatPayload
    : DataFormatPayload<UpdateDataFormatError>
{
    public UpdateDataFormatPayload(
        DataFormat standard
    )
        : base(standard)
    {
    }

    public UpdateDataFormatPayload(
        UpdateDataFormatError error
    )
        : base(error)
    {
    }
}