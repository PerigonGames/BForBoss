using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Perigon.Utility
{
    public class ObjectPooler<T> where T : Component
    {
		protected string name;

		protected List<T> pool = null;

		protected Scene poolScene;

		protected Func<T> createFunc;
		protected Action<T> actionOnGet;
		protected Action<T> actionOnRelease;

        public ObjectPooler(string poolName, Func<T> createFunc, Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            this.name = poolName;
            this.createFunc = createFunc;
            this.actionOnGet = actionOnGet;
            this.actionOnRelease = actionOnRelease;
        }

        public T Get()
		{
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
		}

		public void Reclaim(T toRecycle)
		{
			if (pool == null)
			{
				CreatePools();
			}
			actionOnRelease?.Invoke(toRecycle);
			pool.Add(toRecycle);
		}

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
	}
}
