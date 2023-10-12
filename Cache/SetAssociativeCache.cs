using System;
using System.Collections.Generic;
using System.Threading;

namespace Cache
{
    public class SetAssociativeCache<K, V> where K : IComparable<K>
    {
        private readonly int _maxCount;
        private readonly int _setCount;
        private int Count { get; set; }
        private Set<K, V>[] Sets { get; }

        public SetAssociativeCache(int maxCount, int setCount)
        {
            Validate(maxCount, setCount);

            _maxCount = maxCount;
            _setCount = setCount;
            Sets = new Set<K, V>[setCount];

            for (var i = 0; i < setCount; i++)
            {
                Sets[i] = new Set<K, V>();
            }
        }

        public bool TryAdd(K key, V value)
        {
            if (Count == _maxCount)
            {
                return false;
            }

            var setIdx = Math.Abs(key.GetHashCode()) % _setCount;
            if (!Sets[setIdx].TryAdd(key, value))
            {
                return false;
            }

            Count++;
            return true;
        }

        public bool TryGet(K key, out V value)
        {
            var setIdx = Math.Abs(key.GetHashCode()) % _setCount;
            if (!Sets[setIdx].TryGetValue(key, out value!))
            {
                return false;
            }

            return true;
        }

        public bool TryRemove(K key)
        {
            var setIdx = Math.Abs(key.GetHashCode()) % _setCount;
            if (!Sets[setIdx].TryRemove(key))
            {
                Count--;
                return false;
            }

            return true;
        }

        public void Validate(int maxCount, int setCount)
        {
            if (maxCount <= 0)
            {
                throw new ArgumentException("Max count less or equal to 0 is invalid");
            }

            if (setCount <= 0)
            {
                throw new ArgumentException("Set count less or equal to 0 is invalid");
            }
        }
    }

    internal class Set<K, V> where K : IComparable<K>
    {
        private Dictionary<K, V> Container { get; set; }
        private readonly Mutex _mutex;

        public Set()
        {
            Container = new Dictionary<K, V>();
            _mutex = new Mutex();
        }

        public bool TryAdd(K key, V value)
        {
            _mutex.WaitOne();
            var isSuccess = Container.TryAdd(key, value);
            _mutex.ReleaseMutex();

            return isSuccess;
        }

        public bool TryGetValue(K key, out V value)
        {
            _mutex.WaitOne();
            var isSuccess = Container.TryGetValue(key, out value!);
            _mutex.ReleaseMutex();

            return isSuccess;
        }

        public bool TryRemove(K key)
        {
            _mutex.WaitOne();
            var isSuccess = Container.Remove(key);
            _mutex.ReleaseMutex();

            return isSuccess;
        }
    }
}
