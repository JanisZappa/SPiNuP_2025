using Clips;
using TMPro;
using UnityEngine;


public class Height : MonoBehaviour 
{
    private static TextMeshProUGUI heightText;
    private static int height, zoom, seed;

    public  const float Factor = 10 / 3f;
    private const float Multi  = 1f / Factor;

    
    private void Awake()
    {
        heightText = GetComponent<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        ResetHeight();
        GTime.onPaused += onPaused;
    }


    private void OnDisable()
    {
        GTime.onPaused -= onPaused;
    }
    

    private void LateUpdate()
    {
        if(!GameManager.Running || SaveMeshFrameRange.Saving)
            return;

        Clip clip = Spinner.CurrentPlayerClip;
        if (clip == null)
            return;

        bool update = false;
        int charHeight = Mathf.FloorToInt((clip.BasicPlacement(GTime.Now).pos.y - clip.spinner.size.y * .5f) * Multi);
        if (charHeight > height)
        {
            height = charHeight;
            update = true;
        }
        
        int currentZoom = Mathf.RoundToInt(Mathf.Abs(GameCam.CurrentDolly));
        if (zoom != currentZoom)
        {
            zoom = currentZoom;
            update = true;
        }
        
        int currentSeed = HouseGen.Seed;
        if (seed != currentSeed)
        {
            seed = currentSeed;
            update = true;
        }
        
        if(update)
            heightText.text = "Seed " + seed + " | Zoom " + zoom.PrepString().PadLeft(3) + " | Height " + height.PrepString().PadRight(3) ;
    }


    public static void ResetHeight()
    {
        height = 0;
        if(heightText != null)
            heightText.text = height.PrepString();
    }
    
    
    private static void onPaused(bool paused)
    {
        heightText.enabled = !paused;
    }
}
