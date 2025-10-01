using System.Collections.Generic;
using Clips;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;
using GameModeStuff;


public partial class Spinner 
{
    public const int Count = 4;
    private static readonly Spinner[] spinner = new Spinner[Count];

    private static readonly List<Spinner> inactive = new List<Spinner>(Count);
    public  static readonly List<Spinner> active   = new List<Spinner>(Count);

    private static Spinner focus;

    public static Clip CurrentPlayerClip, CurrentFocusClip;

    private static readonly Item[] CurrentGrabItems = new Item[Count];
    
    public static bool SetGameOver;
    
    
    public static void GameLoad()
    { 
        Object mRig = Resources.Load("Rig");

        for (int i = 0; i < Count; i++)
        {
            Rig rig = (Object.Instantiate(mRig) as GameObject).GetComponent<Rig>();
                    rig.name = "Rig " + i;
                    rig.transform.SetParent(SceneLocator.Characters); 
            
            spinner[i] = new Spinner(i, rig);
        }
    }

    
    public static Spinner Get(int spinnerID)
    {
        if (spinnerID >= Count)
        {
            Debug.Log("SpinnerID too big for getting: " + spinnerID);
            spinnerID = 0;
        }
        return spinner[spinnerID];
    }
    
    
    public static void AllStateUpdate()
    {
        int activeCount = active.Count;
        for (int i = 0; i < activeCount; i++)
            active[i].StateUpdate(active[i].isPlayer && KeyMap.CheckInput());

        if (SetGameOver)
        {
            GameManager.ChangeState(GameState.GameOver);
            SetGameOver = false;
        }

        activeCount = active.Count;

        
    //  Check GrabItems  //
        for (int i = 0; i < activeCount; i++)
            CurrentGrabItems[active[i].ID] = active[i].currentClip is Swing swing? swing.GetStick(GTime.Now).Item : null;
            
            
    //  Controll Focus  //
        focus = null;
        if (activeCount > 0 && focus == null)
            focus = active[0];

        if (activeCount > 1 && Input.GetKeyDown(KeyCode.Tab))
            for (int i = 0; i < activeCount; i++)
                if (active[i] == focus)
                {
                    focus = active[(i + 1) % activeCount];
                    break;
                }

        CurrentFocusClip = focus?.currentClip;
        
        
    //  Dev Buttons  //
        if(KeyMap.Down(Key.Dev_CostumeColor))
            for (int i = 0; i < activeCount; i++)
                active[i].rig.ColorizeMesh();
        
        AllPoseUpdate();
        
        Trimming();
    }

    
    private static void AllPoseUpdate()
    {
        int activeCount = active.Count;
        
        Profiler.BeginSample("Spinner.AllPoseUpdate()");
        
        for (int i = 0; i < activeCount; i++)
            active[i].PoseRig();
        
        Profiler.EndSample();
    }


    private static void Trimming()
    {
        Profiler.BeginSample("Tape Trimming");
        float rewindTime = ShakeRewind.CheckTime - GTime.RewindTime;
        
        int activeCount = active.Count;

        for (int i = 0; i < activeCount; i++)
            active[i].Trimm(rewindTime, false);
        
        Profiler.EndSample();
    }
    
    
    public static void UpdateOffsets(Spinner spinner)
    {
        int activeCount = active.Count;
        
        for (int i = 0; i < activeCount; i++)
            if(active[i] != spinner)
                if(active[i].currentClip is Jump jump)
                    jump.UpdateOffset();
    }


    public static Spinner GetPlayer
    {
        get
        {
            int activeCount = active.Count;
            for (int i = 0; i < activeCount; i++)
                if (active[i].isPlayer)
                    return active[i];

            return null;
        }
    }


    public static Swing GetSwingClip(int spinnerID, Item item)
    {
        return CurrentGrabItems[spinnerID] == item ? spinner[spinnerID].currentClip as Swing : null;
    }


    public static bool IsBeingGrabbed(Item item)
    {
        for (int i = 0; i < Count; i++)
            if (CurrentGrabItems[i] == item)
                return true;

        return false;
    }


    public static void ToggleSkins(bool show)
    {
        for (int i = 0; i < Count; i++)
            spinner[i].rig.ToggleSkin(show);
    }


    public static void SetColorScheme(CostumeColors costume)
    {
        active[0].rig.SetCostume(costume);
    }
}
