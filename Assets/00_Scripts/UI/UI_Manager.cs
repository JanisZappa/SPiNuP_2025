using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;


public enum UIType { Clock, Frame, PoolInfo, ActorInfo, LevelList, MainMenu, SpinUp, Creator }


public static class UI_Manager
{
    static UI_Manager()
    {
        Hits     = new List<RaycastResult>(100);
        activeUI = new List<ActiveUI>(10);
        
        int uiCount = Enum.GetNames(typeof(UIType)).Length;
        uis = new GameObject[uiCount];
        uiResources = new []
        {
            "Debug/Clock_Canvas",
            "Debug/Frame_Canvas",
            "Debug/PoolDebug_Canvas",
            "Debug/ActorDebug_Canvas",
            "UI/LevelList_Canvas",
            "UI/MainMenu_Canvas",
            "UI/SpinUp_Canvas",
            "UI/Creator_Canvas"
        };
    }
    
    public  static bool HitUI;
    private static readonly List<RaycastResult> Hits;
    public  static readonly List<ActiveUI> activeUI;
    
    private static readonly GameObject[] uis;
    private static readonly string[] uiResources;

    private static EventSystem system;
    private static PointerEventData pointer;

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void GetSystem()
    {
        system  = EventSystem.current;
        pointer = new PointerEventData(system);
    }
    
    
    public static void CheckUI()
    {
        HitUI = false;

        Hits.Clear();
        pointer.position = Input.mousePosition;
        system.RaycastAll(pointer, Hits);

        int click = Input.GetMouseButtonDown(0)? 1 : Input.GetMouseButtonDown(1)? 2 : 0;

        int count = activeUI.Count;
        for (int i = 0; i < count; i++)
            if (activeUI[i].HitUI(click))
            {
                HitUI = true;
                click = 0;

                if (activeUI.Count < count)
                {
                    count--;
                    i--;
                }
            }
    }


    public static bool ImPointedAt(GameObject gameObject)
    {
        for (int i = 0; i < Hits.Count; i++)
            if (Hits[i].gameObject == gameObject)
                return true;

        return false;
    }
    
    
    public static GameObject ShowUI(UIType type, bool show, bool fade = false)
    {
        if (fade)
        {
            switch (type)
            {
                case UIType.SpinUp:
                case UIType.Creator:
                    Fade.Transition(v=> ShowUI(type, v), v=> ShowUI(UIType.MainMenu, v));
                    break;
                
                case UIType.MainMenu:
                    Fade.Transition(v=> ShowUI(UIType.MainMenu, v), v=> showFrame.Set(v));
                    break;
            } 
            
            return null;
        }
        
        
        int index = (int)type;
        GameObject current = uis != null && index < uis.Length - 1? uis[index] : null;
        if (show)
        {
            if(!current)
                uis[index] = uiResources[index].ResourceInst(SceneLocator.UI);
            
            return uis[index];
        }
        
        if(current)
            Object.Destroy(current);
        
        return null;
    }
    
    private static readonly BoolSwitch showFrame = new ("UI/Frame", false, b =>
    {
        ShowUI(UIType.Frame, b);
    });
}
