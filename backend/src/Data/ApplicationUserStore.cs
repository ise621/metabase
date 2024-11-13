using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Data;

public sealed class ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null) : UserStore<User, Role, ApplicationDbContext, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>(context, describer)
{

    // Inspired by https://github.com/dotnet/aspnetcore/blob/ef09c065b96af01afa38b82e5b8a35a718685a48/src/Identity/EntityFrameworkCore/src/UserStore.cs#L167C5-L185C6
    // and
    // https://github.com/dotnet/aspnetcore/issues/5840#issue-392371749
    public override async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        // var entry = Context.Entry(user);
        // if (entry.State == EntityState.Detached)
        //     Context.Attach(user);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        // Context.Update(user);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        return IdentityResult.Success;
    }
}