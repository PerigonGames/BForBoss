using System;
using System.Collections.Concurrent;

namespace mixpanel
{
    internal interface IMixpanelPoolable
    {
        void OnRecycle();
    }
    
    internal class Pool<T> where T : IMixpanelPoolable
    {
        private readonly Func<T> _objectGenerator;
        private readonly ConcurrentBag<T> _objects;

        public Pool(Func<T> objectGenerator)
        {
            if (objectGenerator == null)
            {
                throw new ArgumentNullException(nameof(objectGenerator));
            }
            _objectGenerator = objectGenerator;
            _objects = new ConcurrentBag<T>();
        }

        public T Get()
        {
            return _objectGenerator();
        
            // FIXIT: disable the object pooling due to the issues that incorrect object being reused
            // T item;
            // return !_objects.TryTake(out item) ? _objectGenerator() : item;
        }

        public void Put(T item)
        {
            // FIXIT: disable the object pooling due to the issues that incorrect object being reused
            //item.OnRecycle();
            //_objects.Add(item);
        }

        public int Count => _objects.Count;
    }
    
    public static partial class Mixpanel
    {
        internal static readonly Pool<Value> NullPool = new Pool<Value> (() => Value.Null);
        internal static readonly Pool<Value> ArrayPool = new Pool<Value> (() => Value.Array);
        internal static readonly Pool<Value> ObjectPool = new Pool<Value> (() => Value.Object);

        internal static void Put(Value value)
        {
            if (value.IsObject)
            {
                if (ObjectPool.Count < 5000) ObjectPool.Put(value);
                return;
            }
            if (value.IsArray)
            {
                if (ArrayPool.Count < 5000) ArrayPool.Put(value);
                return;
            }
            if (value.IsNull)
            {
                if (NullPool.Count < 100) NullPool.Put(value);
                return;
            }
        }
    }
}
