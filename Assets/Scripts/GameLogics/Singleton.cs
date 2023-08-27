using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    Debug.Log("An instance of " + typeof(T) + " is needed in the scene, but none is found!");
                }
            }
            return _instance;
        }
    }
}