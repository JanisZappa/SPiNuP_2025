using UnityEngine;


public class SwitchScreen : MonoBehaviour
{
    public bool useThisFullscreenRes;
    public Vector2Int res;
    
    private int windowed;
    
    //  0  Fullscreen
    //  1  Window 720p
    //  2  Window 450p
    //  3  Window 225p
    

    private void Start()
    {
        #if !UNITY_EDITOR && UNITY_STANDALONE_OSX
        enabled = false;
        return;
        #endif

        if (Application.isMobilePlatform)
        {
            Destroy(gameObject);
            return;
        }
        
        windowed = Screen.fullScreen? 0 : Screen.height == 720? 1 : Screen.height == 450? 2 : 3;
        
        UpdateRes();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            windowed = (windowed + 1) % 4;

            UpdateRes();
        }
    }


    private void UpdateRes()
    {
        switch (windowed)
        {
            case 0:
                if(useThisFullscreenRes)
                    Screen.SetResolution(res.x, res.y, true);
                else
                    Screen.SetResolution(1920, 1080, true);
                break;
                
            case 1:
                Screen.SetResolution(1280, 720, false);
                break;
                
            case 2:
                Screen.SetResolution(800, 450, false);
                break;
                
            case 3:
                Screen.SetResolution(400, 225, false);
                break;
        }
    }
}
