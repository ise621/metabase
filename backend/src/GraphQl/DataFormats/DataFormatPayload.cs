using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.DataFormats;

public abstract class DataFormatPayload<TDataFormatError>
    : Payload
    where TDataFormatError : IUserError
{
    protected DataFormatPayload(
        DataFormat person
    )
    {
        DataFormat = person;
    }

    protected DataFormatPayload(
        IReadOnlyCollection<TDataFormatError> errors
    )
    {
        Errors = errors;
    }

    protected DataFormatPayload(
        TDataFormatError error
    )
        : this([error])
    {
    }

    protected DataFormatPayload(
        DataFormat person,
        IReadOnlyCollection<TDataFormatError> errors
    )
    {
        DataFormat = person;
        Errors = errors;
    }

    protected DataFormatPayload(
        DataFormat person,
        TDataFormatError error
    )
        : this(
            person,
            [error]
        )
    {
    }

    public DataFormat? DataFormat { get; }
    public IReadOnlyCollection<TDataFormatError>? Errors { get; }
}