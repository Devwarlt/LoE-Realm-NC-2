using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CA.Extensions.Concurrent
{
#pragma warning disable

    public static class ConcurrentDictionaryExtensions
#pragma warning restore
    {
        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist entries.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> EntryWhere<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            foreach (var entry in collection)
                if (predicate.Invoke(entry))
                    yield return entry;
        }

        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist entries in parallel.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey, TValue>[] EntryWhereAsParallel<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Func<KeyValuePair<TKey, TValue>, bool> predicate)
            => collection.AsParallel().Where(_ => predicate.Invoke(_)).Select(_ => _).ToArray();

        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}.Keys"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist keys.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TKey> KeyWhere<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Predicate<TKey> predicate)
        {
            foreach (var entry in collection)
                if (predicate.Invoke(entry.Key))
                    yield return entry.Key;
        }

        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}.Keys"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist keys in parallel.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TKey[] KeyWhereAsParallel<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Predicate<TKey> predicate)
            => collection.AsParallel().Where(_ => predicate.Invoke(_.Key)).Select(_ => _.Key).ToArray();

        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}.Values"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist values.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> ValueWhere<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Predicate<TValue> predicate)
        {
            foreach (var entry in collection)
                if (predicate.Invoke(entry.Value))
                    yield return entry.Value;
        }

        /// <summary>
        /// Provides a predicate check from all <see cref="ConcurrentDictionary{TKey, TValue}.Values"/> entries,
        /// but rather than create a new collection, it'll do a loop through exist values in parallel.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TValue[] ValueWhereAsParallel<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, Predicate<TValue> predicate)
            => collection.AsParallel().Where(_ => predicate.Invoke(_.Value)).Select(_ => _.Value).ToArray();
    }
}
