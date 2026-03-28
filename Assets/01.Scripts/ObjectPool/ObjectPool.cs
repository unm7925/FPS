using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component,IPoolable
{
        private Queue<T> objectPool;

        public T Get()
        {
            T go = objectPool.Dequeue();
            return go;
        }

        public void Return(T obj)
        {
            obj.OnReturn();
            obj.gameObject.SetActive(false);
            objectPool.Enqueue(obj);
        }

        public void Init(T prefab, int count, Transform parent)
        {
            objectPool = new Queue<T>();
            
            for (int i = 0; i < count; i++) 
            {
                T go = GameObject.Instantiate(prefab,parent.position,parent.rotation,null);
                go.gameObject.SetActive(false);
                objectPool.Enqueue(go);
            }
        }
}

