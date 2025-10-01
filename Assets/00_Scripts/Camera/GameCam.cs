using UnityEngine;
using UnityEngine.Profiling;


public class GameCam : MonoBehaviour
{ 
    private static readonly BoolSwitch mapShow = new("Camera/Show Maps", false);
    
    public Camera    cam;
    public Transform camPivot;
    
    [Space(10)]
    
    public SideSwitcher sideSwitcher;
    public MoveCam  moveCam;
    public AngleCam angleCam;
    public PauseCam pauseCam;
    

    public static Vector2 StartPos, CurrentPos;
    public static float   CurrentDolly;
    public static Side    StartSide, CurrentSide;

    public  static Quaternion PivotRot;
    private static Quaternion LocalCamRot;
    
    public static Frustum    frustum;

    public const float gameZoom = -125, closeZoom = -55, farZoom = -250;
    private const float xAngle  = -22,  yAngle    = 44;
    
    private static readonly Quaternion frontRot = Quaternion.Euler(xAngle, -yAngle, 0),
                                        backRot = Quaternion.Euler(xAngle, 180 + yAngle, 0);
    
    private static readonly float frontTilt = RotZ(frontRot), 
                                   backTilt = RotZ(backRot);
    
    public static Camera Cam;
    
    private int cullingMask;
	
    private static float aspectRatio, FOV;
    private static float GetFOV
    {
        get
        {
            if (!f.Same(aspectRatio, ScreenControll.Aspect))
            {
                aspectRatio = ScreenControll.Aspect;
		
                const float portraitFov     = 17,
                    portraitAspect  = 9f / 16f,
                    landscapeAspect = 16f / 9f,
                    radAngle        = portraitFov * Mathf.Deg2Rad;
		
                float lerp    = Mathf.InverseLerp(portraitAspect, landscapeAspect, aspectRatio);
                float radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle * .5f) * portraitAspect);
		
                FOV = Mathf.Lerp(portraitFov, Mathf.Rad2Deg * radHFOV, lerp);
            }
			
            return FOV;
        }
    }
    
    
    public delegate void NewCamSide(bool front);
    public static event NewCamSide OnNewCamSide;
    
    private readonly FloatForce pauseForce = new FloatForce(GPhysics.ForceFPS);
    public static float PauseLerp;
    
    private readonly Vector2Force CamForce = new Vector2Force(GPhysics.ForceFPS);
    
    
    private void Awake()
    {
        Cam = cam;
        frustum = new Frustum(cam);
        
        cam.allowMSAA = true;
		
        cullingMask  = cam.cullingMask;
    }


    public static void SetStartValues(Vector2 pos, float zoom, Side side)
    {
        if (GameManager.IsCreator)
        {
            StartPos   = pos;
            StartSide  = side;
        }
        else
        {
            StartPos   = Level.StartStick.rootPos;
            StartSide  = Level.StartStick.side;
        } 
    }


    public void ResetCamera()
    {
             moveCam.Reset();
        sideSwitcher.Reset();
            angleCam.Reset();
            
        pauseForce.SetSpeed(250).SetDamp(16).SetValue(0).SetForce(0);
        CamForce.SetSpeed(120).SetDamp(12);
        
        CamUpdate(true);
    }
    
    
    public void CameraUpdate() 
    {
        Profiler.BeginSample("GameCam.CameraUpdate()");

        if (GameManager.IsCreator && UI_Creator.SavingOrLoading)
        {
            Profiler.EndSample();
            return;
        }
        
        sideSwitcher.CamUpdate();
            angleCam.CamUpdate();
             moveCam.CamUpdate();
            pauseCam.CamUpdate();
        
        CamUpdate();
        
        Profiler.EndSample();
    }

    
    private void CamUpdate(bool sendSideEvent = false)
    {
        PauseLerp = pauseForce.Update(MoveCam.KeyMove? 0 : GTime.Paused? 1 : 0, Time.deltaTime);

        if (false)
        {
            CurrentDolly = Mathf.LerpUnclamped(  moveCam.dolly,  closeZoom, PauseLerp);
            CurrentPos   = Vector2.LerpUnclamped(moveCam.movePos, pauseCam.movePos, PauseLerp);
        }
        else
        {
            CurrentDolly = moveCam.dolly;
            CurrentPos = moveCam.movePos;
        }
        
        LocalCamRot = Quaternion.Euler(0, 0, Mathf.Lerp(frontTilt, backTilt, sideSwitcher.sideLerp));
        PivotRot    = Quaternion.Slerp(frontRot, backRot, sideSwitcher.sideLerp);
        
        Quaternion sideRot     = PauseCam.Flip? Rot.Y180 : Rot.Zero;
        camPivot.localRotation = sideRot * (angleCam.pivotRot * LocalCamRot);
        
        Vector3 pivotOffset   = new Vector3(0, 0, CurrentDolly);
        float  sideLerp       = GTime.Paused ? CurrentSide.front ? -1 : 1 : Mathf.Lerp(-1, 1, sideSwitcher.sideLerp);
        Vector3 localPivotPos = new Vector3(0, 0, (Level.WallDepth + Level.PlaneOffset) * sideLerp);
    
        camPivot.localPosition = (localPivotPos + angleCam.pivotRot * pivotOffset).MultiXZ(PauseCam.Flip ? -1 : 1);

        if (sendSideEvent)
            CamForce.SetValue(CurrentPos).SetForce(Vector2.zero);
        
        transform.position = CurrentPos;//CamForce.Update(CurrentPos, Time.deltaTime);
        
        float localXAngle = Mathf.LerpUnclamped(ScreenControll.Landscape? -.4f : -1.2f, 0, PauseLerp);
        cam.transform.localRotation = Quaternion.AngleAxis(localXAngle, Vector3.right);
        
    //  Update Side and Height  //
        bool frontBefore = CurrentSide.front;
        bool frontNow    = camPivot.position.z < 0;
        bool newSide     = frontNow != frontBefore;

        if (newSide)
            CurrentSide = frontNow? Side.Front : Side.Back;
            
    //  Update FOV and Culling Masks  //
        cam.fieldOfView = GetFOV;
    
        if (!mapShow)
        {
            cullingMask |=   CurrentSide.front? Layers.MainA_bit : Layers.MainB_bit;
            cullingMask &= ~(CurrentSide.front? Layers.MainB_bit : Layers.MainA_bit);
            cam.cullingMask = cullingMask;
        }
        else
            cam.cullingMask = Layers.Lighting_bit;
        
    //  Side Event  //
        if(newSide || sendSideEvent)
            OnNewCamSide?.Invoke(frontNow);
    }


    public const float MaxDot = .9f; //1.15f;
    private static float RotZ(Quaternion rot)
    {
        float   dot         = Vector3.Dot(rot * V3.forward, V3.forward);
        Vector3 flatForward = Vector3.ProjectOnPlane(rot * V3.forward, V3.forward).normalized;
        float   tiltLerp    = Mathf.Abs(Vector3.Dot(V3.up.RotZ(45), flatForward));
		
        return Mathf.Lerp(5f, -5f, tiltLerp) * Mathf.SmoothStep(0, 1, (1 - Mathf.Abs(dot)) / MaxDot) * (dot > 0? -1 : 1);
    }

    
#if UNITY_EDITOR
    public static void CheckThisOut(Vector3 pos, bool front)
    {
        MoveCam.CheckThisOut(pos);
        
        if(front != CurrentSide.front)
            FindObjectOfType<GameCam>().sideSwitcher.ForcedSideSwitch();
    }
#endif
}