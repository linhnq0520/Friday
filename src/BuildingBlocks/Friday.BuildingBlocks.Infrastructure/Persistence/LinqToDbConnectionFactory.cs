using LinqToDB;
using LinqToDB.Data;

namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public sealed class LinqToDbConnectionFactory(string connectionString) : ILinqToDbConnectionFactory
{
    private readonly string _connectionString = connectionString;
    private DataConnection? _connection;

    public DataConnection GetOrCreateConnection()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        DataOptions options = new DataOptions().UseSqlServer(_connectionString);
        _connection = new DataConnection(options);
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }

    public ValueTask DisposeAsync()
    {
        if (_connection is null)
        {
            return ValueTask.CompletedTask;
        }

        ValueTask disposeTask = _connection.DisposeAsync();
        _connection = null;
        return disposeTask;
    }
}
