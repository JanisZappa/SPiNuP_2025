using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _inst;
    public static T Inst
    {
        get
        {
            if(!_inst)
                _inst = FindObjectOfType<T>();
            
            return _inst; 
        }
    }

    

    private void Awake()
    {
        _inst = FindObjectOfType<T>();
    }
}
