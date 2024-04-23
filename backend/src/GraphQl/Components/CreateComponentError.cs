using System.Collections.Generic;

namespace Metabase.GraphQl.Components
{
    public sealed class CreateComponentError
        : GraphQl.UserErrorBase<CreateComponentErrorCode>
    {
        public CreateComponentError(
            CreateComponentErrorCode code,
            string message,
            IReadOnlyList<string> path
        )
            : base(code, message, path)
        {
        }
    }
}