using Friday.BuildingBlocks.Application.Abstractions;
using Friday.Modules.Salon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Friday.Modules.Salon.Infrastructure;

internal sealed class SalonUnitOfWork(SalonDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null || dbContext.Database.CurrentTransaction is not null)
        {
            return;
        }

        if (!dbContext.Database.IsRelational())
        {
            return;
        }

        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);

        if (_transaction is null)
        {
            return;
        }

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
            return;
        }

        dbContext.ChangeTracker.Clear();
    }
}
