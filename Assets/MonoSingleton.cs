using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
{
    public static T Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else
        {
            DestroyImmediate(this);
        }
    }
}
