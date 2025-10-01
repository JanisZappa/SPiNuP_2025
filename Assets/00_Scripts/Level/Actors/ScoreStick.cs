using System.Collections.Generic;
using Clips;
using LevelElements;
using UnityEngine;


public partial class ScoreStick : Stick 
{
    [Space(10)]
    public MeshFilter stick;
    public Mesh[] stickLevelMeshes, stickPlayerMeshes;
    
    [Space(10)]
    public MeshFilter nob;
    public Mesh[] nobLevelMeshes, nobPlayerMeshes;

    private static readonly byte[] playerColors = { 18 }, 
                                   levelColors  = { 19, 20, 21, 22, 23, 24, 25 };
    

    private byte currentColor;
    private int levelColor;
    
    
    public override void SetItem(Item item)
    {
        base.SetItem(item);
        

        if (item != null)
        {
            const float multi = 1f / 15;
            levelColor = (int) Mathf.Repeat(item.rootPos.y * multi + 1, stickLevelMeshes.Length);
            SetColor();
        }
    }

    
    public override void SetTransform(bool forcedUpdate)
    {
       base.SetTransform(forcedUpdate);
       SetColor();
    }
    
    
    private void SetColor()
    {
        int  player    = states[item.ID].GetPlayer(GTime.Now);
        bool usePlayer = player != -1;
        byte useColor  = usePlayer ? playerColors[player % playerColors.Length] : levelColors[levelColor];


        if (currentColor == useColor)
            return;
        
        currentColor = useColor;

        stick.mesh = usePlayer ? stickPlayerMeshes[player] : stickLevelMeshes[levelColor];
          nob.mesh = usePlayer ?   nobPlayerMeshes[player] :   nobLevelMeshes[levelColor];
    }
}




public partial class ScoreStick
{
    static ScoreStick()
    {
        states       = CollectionInit.Array<State>(Item.TotalCount);
        stateChanges = new List<StateChange>(1000);
    }
    
    private static readonly State[] states;


    private static readonly List<StateChange> stateChanges;
    
    
    public static void ClearStates()
    {
        for (int i = 0; i < Item.TotalCount; i++)
            states[i].Reset();
    }
    

    public static void AddNewState(Swing swingClip)
    {
        Item item = swingClip.startStick.Item;
        
        StateChange change = new StateChange(swingClip);
        if (states[item.ID].AddChange(change))
            stateChanges.Add(change);
    }


    public static void ClearAllAfter(Spinner spinner, float time)
    {
        int start = stateChanges.Count - 1;
        for (int i = start; i > -1; i--)
            if (stateChanges[i].time >= time)
            {
                if (stateChanges[i].spinnerID == spinner.ID)
                {
                    states[stateChanges[i].itemID].RemoveChange(stateChanges[i]);
                    stateChanges.RemoveAt(i);
                    break;
                }
            }
            else
                break;
    }
    
    
    public static void Trimm()
    {
        float trimmTime = GTime.Now - GTime.RewindTime;
        
        int count = stateChanges.Count;
        
        for (int i = 0; i < count; i++)
            if (stateChanges[i].time < trimmTime)
            {
                states[stateChanges[i].itemID].RemoveChange(stateChanges[i]);
                stateChanges.RemoveAt(i);
                count--;
                i--;
            }
            else
                break;
    }


    private class State
    {
        private int firstRecorded, changeCount;
        private readonly StateChange[] changes = new StateChange[1000];

        
        public State()
        {
            Reset();
        }
        
        
        public void Reset()
        {
            changeCount   = 0;
            firstRecorded = -1;
        }

        
        public bool AddChange(StateChange change)
        {
            if (changeCount > 0)
            {
                StateChange lastChange = changes[changeCount - 1];
                if(lastChange.spinnerID == change.spinnerID || lastChange.time >= change.time)
                    return false; 
            }

            changes[changeCount++] = change;
            return true;
        }


        public void RemoveChange(StateChange change)
        {
            for (int i = 0; i < changeCount; i++)
                if (changes[i].Equals(change))
                {
                    if (i == 0)
                        firstRecorded = change.spinnerID;

                    changeCount = changes.ShiftRemoveAt(i, changeCount);
                    break;
                }
        }

        
        public int GetPlayer(float time)
        {
            if (changeCount == 1 && changes[0].time <= time)
                return changes[0].spinnerID;
                
            for (int i = 0; i < changeCount; i++)
                if (changes[i].time > time)
                    return i == 0 ? firstRecorded : changes[i - 1].spinnerID;

            return firstRecorded;
        }
    }


    private struct StateChange
    {
        public  readonly int   itemID, spinnerID;
        public  readonly float time;
        private readonly int   poolID;

        public StateChange(Swing swingClip)
        {
            itemID    = swingClip.startStick.Item.ID;
            spinnerID = swingClip.spinner.ID;
            poolID    = swingClip.poolID;
            time      = swingClip.startTime;
        }
        

        public bool Equals(StateChange other)
        {
            return poolID == other.poolID;
        }
    }
}



