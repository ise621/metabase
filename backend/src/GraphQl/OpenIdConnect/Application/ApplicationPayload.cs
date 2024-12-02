using System.Collections.Generic;
using OpenIddict.EntityFrameworkCore.Models;

namespace Metabase.GraphQl.OpenIdConnect.Application;

public class ApplicationPayload<TApplicationError> : Payload
    where TApplicationError : IUserError
{
    protected ApplicationPayload(
        OpenIddictEntityFrameworkCoreApplication application
    )
    {
        Application = application;
    }

    protected ApplicationPayload(
        IReadOnlyCollection<TApplicationError> errors
    )
    {
        Errors = errors;
    }

    protected ApplicationPayload(
        TApplicationError error
    )
        : this(new[] { error })
    {
    }

    protected ApplicationPayload(
        OpenIddictEntityFrameworkCoreApplication application,
        IReadOnlyCollection<TApplicationError> errors
    )
    {
        Application = application;
        Errors = errors;
    }

    protected ApplicationPayload(
        OpenIddictEntityFrameworkCoreApplication application,
        TApplicationError error
    )
        : this(
            application,
            new[] { error }
        )
    {
    }

    public OpenIddictEntityFrameworkCoreApplication? Application { get; }
    public IReadOnlyCollection<TApplicationError>? Errors { get; }
}