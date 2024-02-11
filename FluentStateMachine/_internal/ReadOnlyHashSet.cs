using System;
using System.Collections;
using System.Collections.Generic;

namespace FluentStateMachine._internal;

internal class ReadOnlyHashSet<T>(IEnumerable<T> collection) : ISet<T>, ICollection<T>, IReadOnlyCollection<T>
#if NET6_0_OR_GREATER
    , IReadOnlySet<T>
#endif
{
    readonly HashSet<T> _set = collection is HashSet<T> hs ? hs : new(collection);

    public bool IsReadOnly => true;
    public int Count => _set.Count;
    IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();
    public bool Contains(T item) => _set.Contains(item);
    public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
    public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);
    public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);

    public bool Add(T item) => throw new NotImplementedException();
    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    public bool Remove(T item) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();
    public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();
    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();
    public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();
}
