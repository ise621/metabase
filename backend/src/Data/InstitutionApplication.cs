using System;

namespace Metabase.Data
{
    public sealed class InstitutionApplication
    {
        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = default!;

        public Guid ApplicationId { get; set; }
        public OpenIdApplication Application { get; set; } = default!;
    }
}