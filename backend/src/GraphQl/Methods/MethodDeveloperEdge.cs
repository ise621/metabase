using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Methods;

public sealed class MethodDeveloperEdge
{
    private readonly InstitutionMethodDeveloperEdge? _institutionMethodDeveloperEdge;
    private readonly UserMethodDeveloperEdge? _userMethodDeveloperEdge;

    public MethodDeveloperEdge(
        InstitutionMethodDeveloperEdge edge
    )
    {
        _institutionMethodDeveloperEdge = edge;
    }

    public MethodDeveloperEdge(
        UserMethodDeveloperEdge edge
    )
    {
        _userMethodDeveloperEdge = edge;
    }

    public async Task<IStakeholder> GetNodeAsync(
        InstitutionByIdDataLoader institutionById,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken
    )
    {
        if (_institutionMethodDeveloperEdge is not null)
            return await _institutionMethodDeveloperEdge.GetNodeAsync(institutionById, cancellationToken)
                .ConfigureAwait(false);

        if (_userMethodDeveloperEdge is not null)
            return await _userMethodDeveloperEdge.GetNodeAsync(userById, cancellationToken).ConfigureAwait(false);

        throw new ArgumentException("Impossible!");
    }

    [UseUserManager]
    public async Task<bool> CanCurrentUserConfirmEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (_institutionMethodDeveloperEdge is not null)
            return await _institutionMethodDeveloperEdge
                .CanCurrentUserConfirmEdgeAsync(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false);

        if (_userMethodDeveloperEdge is not null)
            return await _userMethodDeveloperEdge.CanCurrentUserConfirmEdgeAsync(claimsPrincipal, userManager)
                .ConfigureAwait(false);

        throw new ArgumentException("Impossible!");
    }

    [UseUserManager]
    public async Task<bool> CanCurrentUserRemoveEdgeAsync(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (_institutionMethodDeveloperEdge is not null)
            return await _institutionMethodDeveloperEdge
                .CanCurrentUserRemoveEdgeAsync(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false);

        if (_userMethodDeveloperEdge is not null)
            return await _userMethodDeveloperEdge
                .CanCurrentUserRemoveEdgeAsync(claimsPrincipal, userManager, context, cancellationToken)
                .ConfigureAwait(false);

        throw new ArgumentException("Impossible!");
    }
}