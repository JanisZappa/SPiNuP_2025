using System.Collections.Generic;
using Clips;
using GeoMath;
using UnityEngine;


public class MoveCam : Singleton<MoveCam>
{
    [HideInInspector] public Vector2 charPos, lookAhead, smoothLookAhead, elementLook, smoothElementLook; 
    
    
    private Vector2 keyMovement;
    
    [HideInInspector]
    public float dolly;
    [HideInInspector]
    public Vector2 movePos;

    public float offsetRange = 7.8f;
    public Vector2 offsetMin;
    
    public static Vector2 pos;

    private static bool keyControll;
    public  static bool KeyMove { get { return GameManager.IsCreator || keyControll; }}

    private static bool lookingAtElement;
    private static Link lookLink;


    private static Vector2 GetLookDir
    {
        get
        {
            Vector2 lookDir = (lookLink.b.rootPos - lookLink.a.rootPos) * .4f;
            return lookDir.normalized * Mathf.Min(lookDir.magnitude, 2);
        }
    }
    
    
    public void Reset()
    {
        lookAhead       = V2.zero;
        smoothLookAhead = V2.zero;
        
        if (!KeyMove || GameManager.IsCreator )
        {
            pos = GameCam.StartPos;
            
            dolly = GameCam.gameZoom;
        }
        
        currentWarp = MoveWarp.None;
        warps.Clear();
    }
    
    
    public void CamUpdate()
    {
    //  Dolly  //
        if (!GTime.Paused && !UI_Manager.HitUI)
            dolly = Mathf.Clamp(dolly + Controll.ScrollWheelDelta * 5000, GameCam.farZoom, GameCam.closeZoom);
        
        
    //  Char Values  //
        Clip clip = Spinner.CurrentFocusClip;
        if (clip?.spinner != null)
        {
            Vector2 mV = clip.GetMV(GTime.Now) * GTime.Speed;
                
            lookAhead = Vector2.Lerp(lookAhead, new Vector2(mV.x, mV.y * (mV.y > 0? .8f : 1.13f)), Time.deltaTime * 12);
            charPos   = clip.spinner.currentPlacement.pos.V2() + lookAhead;
        }

        if(clip != null && clip.Type.IsAnyJump())
            lookingAtElement = false;
        
        
        elementLook = Vector2.Lerp(elementLook, lookingAtElement? GetLookDir : V2.zero, Time.deltaTime * 6);
        
        
    //  Movement  //
        if (KeyMap.Down(Key.Dev_CamModeToggle))
            keyControll = !keyControll;

        if (!KeyMove)
            GameMoveUpdate();
        else
            KeyMoveUpdate();
        
        smoothLookAhead   = Vector2.Lerp(smoothLookAhead, lookAhead, Time.deltaTime);
        smoothElementLook = Vector2.Lerp(smoothElementLook, elementLook, Time.deltaTime);
        
        
    //  Damp  //
        {
            float min = ScreenControll.Landscape? offsetMin.x : offsetMin.y;
            float max = min + offsetRange;
            
            Vector2 combined = pos + smoothElementLook;
                    combined = new Vector2(combined.x, Mathf.Max(combined.y, min));
            
            movePos = new Vector2(combined.x, SineEaseOut(combined.y, max, min));
        }
    }


    private void GameMoveUpdate()
    {
    //  Warp Check  //
        CheckForWarpMove();
        pos = Vector2.Lerp(pos, charPos, Time.deltaTime);
    }


    private void KeyMoveUpdate()
    {
        Vector2 currentControll = new Vector2(Input.GetAxis("Horizontal") * (GameCam.CurrentSide.front? 1 : -1), 
                                              Input.GetAxis("Vertical"));

        float accel = !GameManager.IsCreator ?  4 : 18;
        float speed = !GameManager.IsCreator ? 50 : 55;
        keyMovement = Vector2.Lerp(keyMovement, currentControll, Time.deltaTime * accel);
        pos += keyMovement * Time.deltaTime * speed * 1.5f;
        lookAhead = Vector2.zero;
    }
    
    
    private static float SineEaseOut(float value, float start, float end)
    {
        const float multi = Mathf.PI * .5f;
        
        float range = end - start;
        start = end - range * multi;
        float lerp = (value - start)  / (range * multi);

        if (lerp < 0)
            return value;
        
        return start + range * multi * Mathf.Sin(Mathf.Clamp(lerp, 0, multi));
    }


    public static void SetWarp(float time, Vector2 start, Vector2 end)
    {
        //Debug.Log("Setting Warp: " + time);
        MoveWarp warp = new MoveWarp(time, start, end);
        
        int count = warps.Count;
        for (int i = 0; i < count; i++)
            if(warps[i].Equals(warp))
                return;
        
        warps.Add(warp);
    }



    private static void CheckForWarpMove()
    {
        if (warps.Count == 0)
            return;
        
        
    //  Trimm  //
        while (true)
        {
            if (warps[0].time < GTime.Now - GTime.RewindTime * 2)
            {
                warps.RemoveAt(0);
                if(warps.Count > 0)
                    continue;
                
                currentWarp = MoveWarp.None;
                return;
            }
    
            break;
        }

        int warpCount = warps.Count;
        
        MoveWarp newWarp = GetCurrentWarp;

        if (!currentWarp.Equals(newWarp))
        {
            Vector2 warpOffset = Vector2.zero;

            int startIndex = -1;
            for (int i = 0; i < warpCount; i++)
                if (warps[i].Equals(currentWarp))
                {
                    startIndex = i;
                    break;
                }
            
            bool newWarpIsLater = currentWarp.time < newWarp.time;

            Vector2 checkPos;

            if (newWarpIsLater)
            {
                for (int i = startIndex + 1; i < warpCount; i++)
                {
                    warpOffset += warps[i].offset;
                    
                    if(warps[i].Equals(newWarp))
                        break;
                }

                checkPos = newWarp.end;
            }
            else
            {
                for (int i = startIndex; i > -1; i--)
                {
                    if(warps[i].Equals(newWarp))
                        break;
                    
                    warpOffset -= warps[i].offset;
                }
                
                checkPos = currentWarp.start;
            }
            
            currentWarp = newWarp;

            bool onScreen = GameCam.frustum.InFrustum(new Bounds2D(checkPos).Pad(.01f), GameCam.CurrentSide.front);
            
            if(!onScreen)
                pos -= warpOffset;
        }
    }


    private static MoveWarp GetCurrentWarp
    {
        get 
        {  
            int count = warps.Count;
            
            MoveWarp warp = MoveWarp.None; 
            for (int i = 0; i < count; i++)
                if (warps[i].time < GTime.Now)
                    warp = warps[i];
                else
                    break;
                
            return warp; 
        } 
    }

  
    private static MoveWarp currentWarp;


    public static void CheckThisOut(Vector3 checkPoint)
    {
        keyControll = true;
        pos         = checkPoint; 
    }


    public static void LookAt(Link link)
    {
        //Debug.Log("Look at this!!!!");
        lookLink = link;
        lookingAtElement = true;
    }
    
    
    
    public struct MoveWarp
    {
        public readonly float time;
        public Vector2 start, end;

        public Vector2 offset
        {
            get { return start - end; }
        }

        public MoveWarp(float time, Vector2 start, Vector2 end)
        {
            this.time   = time;
            this.start = start;
            this.end   = end;
        }

        /*public static bool operator ==(MoveWarp a, MoveWarp b)
        {
            return f.Same(a.time, b.time);
        }
        public static bool operator !=(MoveWarp a, MoveWarp b)
        {
            return !f.Same(a.time, b.time);
        }*/

        public bool Equals(MoveWarp other)
        {
            return f.Same(time, other.time);
        }
        
        public static MoveWarp None { get { return new MoveWarp(float.MinValue, Vector2.zero, Vector2.zero); } }
    }
    
    
    private static readonly List<MoveWarp> warps = new List<MoveWarp>(100);
}
