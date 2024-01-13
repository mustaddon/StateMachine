using System.Collections.Generic;
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal static partial class IAsyncEnumerableExt
{
    public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var result = new List<T>();

        await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            result.Add(item);

        return result;
    }
}
#endif
