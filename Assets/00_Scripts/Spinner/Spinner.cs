using Anim;
using Clips;
using GeoMath;
using LevelElements;
using UnityEngine;


public partial class Spinner
{
    private Spinner(int ID, Rig rig)
    {
        this.ID = ID;
        this.rig = rig;
        rig.Setup(this);

        size = rig.poser.size.RotY(90);
        
        tape     = new Tape(ID == 0);
        squashes = new Squashes();
        
        Disable();
    }
    
    public readonly int ID;
    public readonly Rig rig;
    
    public string  name;
    public Costume costume;
    public Vector3 size;
    
    public readonly Tape     tape;
    public readonly Squashes squashes;
    
    public bool      visible          { get;  private set; }
    public Clip      currentClip      { get;  private set; }
    public Placement currentPlacement { get;  private set; }

    public float startTime;

    public bool isPlayer;

    public bool isFocus => this == focus;


    public float ConnectRadius(Item item = null)
    {
        return size.y * .5f + (item?.radius ?? Item.DefaultRadius);
    }
    
    
    public float GetZShift(float time)
    {
        /*const float range = 1.3f;

        float closeLerp = Boundary.CloseLerp(ID);
        return ID == 0 ? -.6f * closeLerp : .6f * closeLerp;*/
        return 0;
        /*return Mathf.Sin(time * 2) * 1.5f;
        //return Mathf.Sin(time * 1) * range;
        
        
        switch (ID)
        {
            default:   return      0;
            case 1:    return  range;
            case 2:    return -range;
        }*/
    }

    
    public void Enable(ByteReplay replayData)
    {
        name    = replayData.charName;
        costume = replayData.costume;
        
        replayData.DeserializeClips(this, true);

        isPlayer = false;
       
        SetActive();
    }


    public void Enable(string name, Costume costume)
    {
        this.name    = name;
        this.costume = costume;

        startTime = GTime.LastLoopPoint;
        tape.SetClip(Clip.Get_Clip_Spawn(this, startTime, Level.StartStick.side));
        
        isPlayer = true;

        SetActive();
    }
    
    
    private void SetActive()
    {
        active.AddUnique(this);

        inactive.Remove(this);
        
        rig.poser.SetActive(true);
        
        rig.ColorizeMesh();
    }


    public void Disable()
    {
        //Debug.Log("Disabling " + ID);

        inactive.AddUnique(this);

        active.Remove(this);

        currentClip = null;
        
        if (isPlayer)
            CurrentPlayerClip = null;
        
        if (focus == this)
            focus = null;
        
        tape.Clear();
      
        visible = false;
            
        squashes.ResetAll();
        
        rig.poser.SetActive(false);
    }
    

    private void StateUpdate(bool gotInput)
    {
    //  Input Check  //
        if (gotInput && 
            currentClip != null && 
            currentClip.Type.IsAnySwing())
        {
            Swing swing    = (Swing) currentClip;
            Clip  jumpClip = swing.GetJumpData(GTime.LastFrame, this);
            tape.SetClip(jumpClip);
        }
        
        
    //  ClipCheck  //
        Clip newClip = tape.GetClip(GTime.Now);
        if (currentClip != newClip)
        {
            currentClip = newClip;
            currentClip.Activate();

            if (isPlayer)
                CurrentPlayerClip = currentClip;
        }
        
        
    //  Death Check  //
        if (isPlayer && 
            currentClip != null && 
            currentClip.Type != ClipType.Dead && 
            currentClip.BasicPlacement(GTime.Now).pos.y < 0)
            tape.SetClip(Clip.PoolClip(ClipType.Dead, this, GTime.Now, currentClip.startSide));
    }


    private void PoseRig()
    {
        currentPlacement = currentClip.FinalPlacement(GTime.Now);

        bool isInFrustum = GameCam.frustum.InFrustum(new Bounds2D(currentPlacement.pos).Pad(size.y * .5f), currentClip.GetSide(GTime.Now).front);
        if (visible != isInFrustum)
        {
            visible = !visible;
            rig.poser.SetActive(visible);
        }


        if (visible)
        {
            rig.poser.SetPose(currentPlacement, currentClip.ReadPose(GTime.Now), squashes.GetSquash(GTime.Now));
            rig.debug.Update();
        }
    }


    public void Trimm(float time, bool after = true)
    {
            tape.Trimm(time, after);
        squashes.Trimm(time, after);
    }


    public byte[] GetReplayBytes()
    {
        return tape.GetReplayBytes();
    }
}