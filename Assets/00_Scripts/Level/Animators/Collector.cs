using System.Collections.Generic;
using Clips;
using Future;
using LevelElements;
using UnityEngine;


public static class Collector
{
    static Collector()
    {
        collectionStates = new CollectionState[Item.TotalCount];
        for (int i = 0; i < Item.TotalCount; i++)
            collectionStates[i] = CollectionState.None;
        
        GameManager.OnGameStart += GameManagerOnOnGameStart;
    }

    public static readonly CollectionState[] collectionStates;
    private const float StepLength = JumpInfo.StepLength / 4;
    
    private static readonly List<CollectionState> timelineStates = new List<CollectionState>(100);

    private static int scoreValue;

    private static CollectionState lastState, trimmState;

    
    public static bool IsCollected(Item item)
    {
        return collectionStates[item.ID].time <= GTime.Now;
    }
    

    private static void GameManagerOnOnGameStart()
    {
        for (int i = 0; i < Item.TotalCount; i++)
            collectionStates[i] = CollectionState.None;
        
        timelineStates.Clear();

        scoreValue = 0;
        PillUI.SetPillCount(0);

        lastState  = CollectionState.None;
        trimmState = CollectionState.None;
    }


    public static void GameUpdate()
    {
        int count = timelineStates.Count;
        
        float clearBefore = GTime.Now - GTime.RewindTime;
        for (int i = 0; i < count; i++)
        {
            CollectionState state = timelineStates[i];
            
            if (state.time < clearBefore)
            {
                trimmState = state;
                timelineStates.RemoveAt(i);
                i--;
                count--;
            }
        }
            
        
            
        
    //  Check Pill Count Increase  //
        float bestTime = float.MinValue;
        CollectionState current = trimmState;
        for (int i = 0; i < count; i++)
        {
            CollectionState state = timelineStates[i];

            if (state.time + Coin.CollectionLength <= GTime.Now && state.time > bestTime)
            {
                bestTime = state.time;
                current  = state;
            }
        }

        if (!current.Equals(lastState))
        {
            lastState = current;
            PillUI.SetPillCount(lastState.score);
        }
    }
    
    
    public static void CollectThings(Jump jump)
    {
        JumpInfo info = jump.info;
        int proxyCount = info.proxCount;

        Spinner spinner = jump.spinner;
        float spinnerRadius = spinner.size.y * .5f;

        int statesBefore = timelineStates.Count;

        for (int i = 0; i < proxyCount; i++)
        {
            ProxItem proxItem = info.proxItems[i];
            Item     item     = proxItem.item;
            
            if(!Mask.IsCollectable.Fits(item.elementType) || proxItem.startTime >= collectionStates[item.ID].time)
                continue;

            float triggerDist = Mathf.Pow((spinnerRadius + item.radius) * 4f, 2);

            if (proxItem.closestDist <= triggerDist)
            {
                float checkDist = Mathf.Pow(spinnerRadius + item.radius, 2);

                float end       = proxItem.closestTime + JumpInfo.StepLength;
                float checkTime = Mathf.Max(jump.startTime, proxItem.closestTime - JumpInfo.StepLength * 2);
                
                if (ItemCheck.FirstHit(checkTime, end, StepLength, jump, item, checkDist, out checkTime))
                {
                    CollectionState state = new CollectionState(checkTime, item, spinner);
                    collectionStates[item.ID] = state;
                    timelineStates.Add(state);
                }
            }
        }
        
        //  Order  //
        int count = timelineStates.Count;
        for (int i = statesBefore; i < count; i++)
            if (i < count - 1)
            {
                CollectionState a = timelineStates[i];
                CollectionState b = timelineStates[i + 1];

                if (b.time < a.time)
                {
                    timelineStates[i] = b;
                    timelineStates[i + 1] = a;
                    i = statesBefore - 1;
                }
            }

        for (int i = statesBefore; i < count; i++)
        {
            scoreValue += timelineStates[i].item.elementType.Value();
            timelineStates[i] = timelineStates[i].SetScore(scoreValue);
        }  
    }

    
    public static void ClearAfter(Spinner spinner, float time)
    {
        int start = timelineStates.Count - 1;
        
        for (int i = start; i > -1; i--)
            if (timelineStates[i].CanBeTrimmed(time, spinner))
            {
                scoreValue -= timelineStates[i].item.elementType.Value();
                timelineStates.RemoveAt(i);
            }
            else
                break;
    }
    
    
    public struct CollectionState
    {
        public readonly float time;
        public readonly int score;
        public readonly Item item;
        public readonly Spinner spinner;

        public CollectionState(float time, Item item, Spinner spinner)
        {
            this.time = time;
           
            this.spinner = spinner;
            this.item = item;
            
            score = 0;
        }

        
        private CollectionState(float time, Item item, Spinner spinner, int score)
        {
            this.time    = time;
            this.spinner = spinner;
            this.item    = item;
            this.score   = score;
        }
        

        public bool CanBeTrimmed(float time, Spinner spinner)
        {
            bool trimmIt = this.spinner == spinner && this.time >= time;

            if (trimmIt)
                collectionStates[item.ID] = None;
            
            return trimmIt;
        }
        
        
        public static CollectionState None = new CollectionState(float.MaxValue, null, null);


        public bool Equals(CollectionState other)
        {
            return score == other.score;
        }
        

        public CollectionState SetScore(int score)
        {
            return new CollectionState(time, item, spinner, score);
        }
    }
}
