using System;
using System.ComponentModel.DataAnnotations;
using Metabase.Enumerations;

namespace Metabase.Data;

public sealed class InstitutionRepresentative
{
    public Guid InstitutionId { get; set; }
    public Institution Institution { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [Required] public InstitutionRepresentativeRole Role { get; set; }

    public DataSigningPermission DataSigningPermission { get; set; } = DataSigningPermission.NEVER;

    public bool Pending { get; set; } = true;
}