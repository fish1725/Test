﻿using UnityEngine;
using F.Common;
using System;
using UnityEngine.Assertions;

namespace F.Memory
{
    [AddComponentMenu("F/Singleton/GameObjectPool")]
    public class GameObjectPool : MonoSingleton<GameObjectPool>
    {
        private GameObject _go;
		public ObjectPoolEntry[] ObjectPools = new ObjectPoolEntry[0];

		public GameObject PoolGameObject
		{
			get { return _go; }
		}

		// Called after the singleton instance is created
		private void Awake()
		{
			_go = new GameObject("GameObjects");
			_go.transform.parent = gameObject.transform;

			// Iterate pool entries and create a pool of prefabs for each of them
			foreach (ObjectPoolEntry pool in Instance.ObjectPools)
			{
				if (pool.Prefab == null)
				{
					Debug.LogError("No prefab specified in one of the object pool's entries");
					continue;
				}

				pool.Init(_go, pool.Prefab);
			}
		}

		// Returns a pool of a given name if it exists
		public static ObjectPoolEntry GetPool(string poolName)
		{
			foreach (ObjectPoolEntry pool in Instance.ObjectPools)
			{
				if (pool.Name == poolName)
					return pool;
			}
			return null;
		}

		public static void PushObject(string poolName, GameObject go)
		{
			if (go == null)
				throw new ArgumentNullException(string.Format("Trying to pool a null game object in pool {0}", poolName));

			ObjectPoolEntry pool = GetPool(poolName);
			if (pool == null)
				throw new InvalidOperationException(string.Format("Object pool {0} does not exist", poolName));

			pool.Push(go);
		}

		public static GameObject PopObject(string poolName)
		{
			ObjectPoolEntry pool = GetPool(poolName);
			if (pool == null)
				throw new InvalidOperationException(string.Format("Object pool {0} does not exist", poolName));

			return pool.Pop();
		}

		public override string ToString()
		{
            string s = "ObjectPools ";
			foreach (var entry in ObjectPools)
                s += string.Format("{0},", entry.ToString());
			return s;
		}

		[Serializable]
		public class ObjectPoolEntry
		{
			public string Name;
			public GameObject Prefab;
			public int InitialSize = 128;

			[HideInInspector] public ObjectPool<GameObject> Cache;

			private GameObject m_parentGo;

			public ObjectPoolEntry()
			{
				//Name = "";
				//InitialSize = 0;
				m_parentGo = null;
				Prefab = null;
				Cache = null;
			}

			public void Init(GameObject parentGo, GameObject prefab)
			{
				m_parentGo = parentGo;
				Prefab = prefab;

				Cache = new ObjectPool<GameObject>(
					arg =>
					{
						GameObject newGO = Instantiate(Prefab);
						newGO.name = Prefab.name;
						newGO.SetActive(false);
						newGO.transform.parent = m_parentGo.transform; // Make this object a parent of the pooled object
						return newGO;
					},
					InitialSize,
					false
					);
			}

			public void Push(GameObject go)
			{
				// Deactive object, reset its' transform and physics data
				go.SetActive(false);

				Rigidbody rbody = go.GetComponent<Rigidbody>();
				if (rbody != null)
					rbody.velocity = Vector3.zero;

				// Place a pointer to our object to the back of our cache list
				Cache.Push(go);
			}

			public GameObject Pop()
			{
				GameObject go = Cache.Pop();

				// Reset transform and active it
				//go.transform.parent = null;
				Assert.IsTrue(!go.activeSelf, "Popped an active gameObject!");
				go.SetActive(true);

				return go;
			}

			public override string ToString()
			{
				return string.Format("{0}={1}", Name, Cache);
			}
		}
    }
}