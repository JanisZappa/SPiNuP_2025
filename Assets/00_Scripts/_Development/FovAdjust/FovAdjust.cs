using UnityEngine;


public class FovAdjust : MonoBehaviour
{
    public bool orthographic;
    public float defaultSize;
    public float defaultFov;
    public float defaultAspect;

    public Camera cam;
    private Camera Cam
    {
        get
        {
            if (cam == null)
               GetCameraComponent();

            return cam;
        }
    }

    private float Size{ get { return Cam == null ? defaultSize : defaultSize * (defaultAspect / Cam.aspect); }}

    private float FOV
    {
        get
        {
            if (Cam == null)
                return defaultFov;

            float hFov = 2 * Mathf.Atan(defaultAspect * Mathf.Tan(defaultFov * .5f * Mathf.Deg2Rad));
            return 2 * Mathf.Atan(Mathf.Tan(hFov * .5f) / Cam.aspect) * Mathf.Rad2Deg;
        }
    }

    
    public void SetDefaultValues()
    {
        orthographic = Cam.orthographic;
        defaultSize = Cam.orthographicSize;
        defaultFov = Cam.fieldOfView;
        defaultAspect = Cam.aspect;
    }


    private void GetCameraComponent()
    {
        cam = GetComponent<Camera>();
    }
   
    
    private void OnEnable()
    {
        AdjustCam();
    }


    public void AdjustCam()
    {
        if (orthographic)
            Cam.orthographicSize = Size;
        else
            Cam.fieldOfView = FOV;
    }
}

