using UnityEngine;

public class Singleton<T> where T : class, new()
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }
}

public class SingletonObj<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj;

                obj = GameObject.Find(typeof(T).Name);

                if(obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
                else
                {
                    _instance = obj.GetComponent<T>();
                }

                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }
}
