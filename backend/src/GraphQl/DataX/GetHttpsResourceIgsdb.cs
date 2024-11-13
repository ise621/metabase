using System;

namespace Metabase.GraphQl.DataX;

public sealed class GetHttpsResourceIgsdb(
    Uri locator
    )
{
    public Uri Locator { get; } = locator;
}