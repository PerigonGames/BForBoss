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

		protected Func<T> buildObject;
		protected Action<T> actionOnGet;
		protected Action<T> actionOnRelease;

		/// <summary>
		/// Create a new Object pooler for a given type
		/// </summary>
		/// <param name="poolName">Name of the pool, to be used for the pool scene name</param>
		/// <param name="buildObject">Function that creates a new instance of an object when existing object is available</param>
		/// <param name="actionOnGet">Setup action on grabbing a new object from the pool</param>
		/// <param name="actionOnRelease">Clean up action when recycling an object</param>
		public ObjectPooler(string poolName, Func<T> buildObject, Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
		{
			this.name = poolName;
			this.buildObject = buildObject;
			this.actionOnGet = actionOnGet;
			this.actionOnRelease = actionOnRelease;

#if UNITY_2021_1_OR_NEWER
			objectPool = new ObjectPool<T>(buildObject, actionOnGet, actionOnRelease);
#else 
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
#endif
		}

		~ObjectPooler()
        {
#if UNITY_2021_1_OR_NEWER
			objectPool.Clear();
#else
			SceneManager.UnloadSceneAsync(poolScene); //destroys all objects in the pool
			pool = null;
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
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
				instance = buildObject();
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
#if UNITY_EDITOR
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
#endif
			poolScene = SceneManager.CreateScene(name);
		}
#endif

#if !UNITY_2021_1_OR_NEWER
	    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
	    {

		    pool = null;

	    }
#endif
	}
}
