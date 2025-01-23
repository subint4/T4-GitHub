using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if(applicationIsQuitting)
            {
                Debug.LogWarning($"싱글턴 인스턴스{typeof(T)}가 이미 삭제되었습니다. null을 반환합니다");
                return null;
            }
            if (instance == null)
            {
                instance=(T)FindObjectOfType(typeof(T));

                if(FindObjectsOfType(typeof(T)).Length > 1 )
                {
                    Debug.LogError($"문제 발생 싱글턴 인스턴스가 1개 이상입니다." );
                    return instance;
                }
                if(instance == null)
                {
                    GameObject go = new GameObject();
                    instance = go.AddComponent<T>();
                    go.name = $"{typeof(T).Name}";
                }
            }
            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        applicationIsQuitting=true;
    }
}
