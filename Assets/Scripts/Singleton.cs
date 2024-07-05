using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Patterns
{


    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T singleton_instance;

        public static T Instance
        {
            get
            {
                if (singleton_instance == null)
                {
                    singleton_instance = FindObjectOfType<T>();
                    if (singleton_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        singleton_instance = obj.AddComponent<T>();
                    }

                }
                return singleton_instance;
            }
        }
        protected void Awake()
        {
            if(singleton_instance == null) 
            {
                singleton_instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}