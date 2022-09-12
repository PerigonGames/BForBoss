using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Perigon.Utility
{
    public class ObjectPooler<T> where T : Component
    {
	    protected IObjectPool<T> objectPool;
		protected Func<T> buildObject;
		protected Action<T> actionOnGet;
		protected Action<T> actionOnRelease;

		/// <summary>
		/// Create a new Object pooler for a given type
		/// </summary>
		/// <param name="buildObject">Function that creates a new instance of an object when existing object is available</param>
		/// <param name="actionOnGet">Setup action on grabbing a new object from the pool</param>
		/// <param name="actionOnRelease">Clean up action when recycling an object</param>
		public ObjectPooler(Func<T> buildObject, Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
		{
			this.buildObject = buildObject;
			this.actionOnGet = actionOnGet;
			this.actionOnRelease = actionOnRelease;
			objectPool = new ObjectPool<T>(buildObject, actionOnGet, actionOnRelease);
		}

		~ObjectPooler()
        {
	        objectPool.Clear();
        }

		public T Get()
		{
			return objectPool.Get();
		}

		public void Reclaim(T toRecycle)
		{
			objectPool.Release(toRecycle);
		}

		public void Clear()
		{
			objectPool.Clear();
		}
    }
}
