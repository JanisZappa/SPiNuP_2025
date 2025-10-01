using UnityEngine;


public abstract class ActiveUI : MonoBehaviour
{
    public virtual void OnEnable()
    {
        UI_Manager.activeUI.Add(this);
    }
    
    public virtual void OnDisable()
    {
        UI_Manager.activeUI.Remove(this);
    }
    
    public virtual void OnDestroy()
    {
        UI_Manager.activeUI.Remove(this);
    }
    
    public abstract bool HitUI(int click);
}
