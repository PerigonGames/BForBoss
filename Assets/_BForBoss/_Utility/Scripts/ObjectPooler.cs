using System;
using UnityEngine;
#if UNITY_2021_1_OR_NEWER
using UnityEngine.Pool;
#else
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#endif

namespace Perigon.Utility
{
    public class ObjectPooler<T> where T : Component
    {
		protected string name;

#if UNITY_2021_1_OR_NEWER
		protected IObjectPool<T> objectPool;
#else
		protected Scene poolScene;
		protected List<T> pool = null;
#endif

		protected Func<T> createFunc;
		protected Action<T> actionOnGet;
		protected Action<T> actionOnRelease;

        public ObjectPooler(string poolName, Func<T> createFunc, Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            this.name = poolName;
            this.createFunc = createFunc;
            this.actionOnGet = actionOnGet;
            this.actionOnRelease = actionOnRelease;

#if UNITY_2021_1_OR_NEWER
			objectPool = new ObjectPool<T>(createFunc, actionOnGet, actionOnRelease);
#endif
		}

        public T Get()
		{

#if UNITY_2021_1_OR_NEWER
			return objectPool.Get();
#else
			T instance;
			if (pool == null)
			{
				CreatePools();
			}
			int lastIndex = pool.Count - 1;
			if (lastIndex >= 0)
			{
				instance = pool[lastIndex];
				pool.RemoveAt(lastIndex);
			}
			else
			{
				instance = createFunc();
				SceneManager.MoveGameObjectToScene(
					instance.gameObject, poolScene
				);
			}
			actionOnGet?.Invoke(instance);
			return instance;
#endif
		}

		public void Reclaim(T toRecycle)
		{
#if UNITY_2021_1_OR_NEWER
			objectPool.Release(toRecycle);
#else
			if (pool == null)
			{
				CreatePools();
			}
			actionOnRelease?.Invoke(toRecycle);
			pool.Add(toRecycle);
#endif
		}

#if !UNITY_2021_1_OR_NEWER
		void CreatePools()
		{
			pool = new List<T>();

			if (Application.isEditor)
			{
				poolScene = SceneManager.GetSceneByName(name);
				if (poolScene.isLoaded)
				{
					GameObject[] rootObjects = poolScene.GetRootGameObjects();
					for (int i = 0; i < rootObjects.Length; i++)
					{
						T instance = rootObjects[i].GetComponent<T>();
						if (!instance.gameObject.activeSelf)
						{
							pool.Add(instance);
						}
					}
					return;
				}
			}

			poolScene = SceneManager.CreateScene(name);
		}
#endif
	}
}
