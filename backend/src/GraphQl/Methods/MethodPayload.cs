using System.Collections.Generic;
using Metabase.Data;

namespace Metabase.GraphQl.Methods;

public abstract class MethodPayload<TMethodError>
    : Payload
    where TMethodError : IUserError
{
    protected MethodPayload(
        Method method
    )
    {
        Method = method;
    }

    protected MethodPayload(
        IReadOnlyCollection<TMethodError> errors
    )
    {
        Errors = errors;
    }

    protected MethodPayload(
        TMethodError error
    )
        : this([error])
    {
    }

    protected MethodPayload(
        Method method,
        IReadOnlyCollection<TMethodError> errors
    )
    {
        Method = method;
        Errors = errors;
    }

    protected MethodPayload(
        Method method,
        TMethodError error
    )
        : this(
            method,
            [error]
        )
    {
    }

    public Method? Method { get; }
    public IReadOnlyCollection<TMethodError>? Errors { get; }
}