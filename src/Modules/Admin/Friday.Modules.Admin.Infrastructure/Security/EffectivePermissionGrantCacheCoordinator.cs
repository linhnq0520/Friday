using System.Threading;
using Friday.Modules.Admin.Application.Security;

namespace Friday.Modules.Admin.Infrastructure.Security;

public sealed class EffectivePermissionGrantCacheCoordinator
    : IEffectivePermissionGrantCacheCoordinator
{
    private long _generation;

    public long CurrentGeneration => Volatile.Read(ref _generation);

    public void InvalidateAllGrants() => Interlocked.Increment(ref _generation);
}
