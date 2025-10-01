using System.Text;
using Clips;
using LevelElements;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InfoPanelUpdate : MonoBehaviour
{
    public TextMeshProUGUI textL, textM, textR;
    private readonly StringBuilder buildL = new(200), buildM = new(200), buildR = new(200);
    
    private float alpha;
    private static float delta;

    
    private void OnEnable()
    {
        delta = Time.deltaTime;
    }
    
    
    private void Update()
    {
        if (!GameManager.Running)
            return;
        
        delta = Mathf.Lerp(delta, Time.deltaTime, Time.deltaTime * 10);
        
        buildL.Length = buildM.Length = buildR.Length = 0;

        if (!GameManager.IsCreator)
        {
            textL.text = buildL.Append("").Append("\n").Append(GTime).Append("\n").Append(Loop).Append("\n").Append(FPS).ToString();
            textM.text = buildM.Append(ClipType).Append("\n").Append("").Append("\n").Append("").Append("\n").Append(ClipSide).ToString();
            textR.text = buildR.Append("").Append("\n").Append("").Append("\n").Append(ActiveSticks).Append("\n").Append(Zoom).ToString();
        }
        else
        {
            textL.text = buildL.Append(GTime).Append("\n").Append(Loop).Append("\n").Append(FPS).Append("\n").Append("").ToString();
            textM.text = buildM.Append("").ToString();
            textR.text = buildR.Append(StickCount).Append("\n").Append(ActiveSticks).Append("\n").Append("").Append("\n").Append(Zoom).ToString();
        }
    }
    
    
    private static string GTime => "Time: " + global::GTime.Now.ToString("F2");

    private static string Loop =>
        "Loop " + global::GTime.LoopTime.ToString("F2") + ": " + 
        Mathf.FloorToInt(global::GTime.LoopFraction * 100).ToString("D2");

    private static string FPS => "FPS: " + Mathf.CeilToInt(Mathf.Clamp(1f / delta, 0, 60)) + "/" + 60;

    private static string Zoom => "CamZ: " + GameCam.CurrentDolly.ToString("F0");

    private static ClipType CurrentClipType => GameManager.Running ? Clip.Type : 0;

    private static Clip   Clip         => Spinner.CurrentFocusClip;
    private static string ClipType     => "Clip: " + CurrentClipType;
    private static string ClipSide     => "Side: " + (GameManager.Running ? Clip.GetSide(global::GTime.Now) : new Side(true));

    private static string StickCount   => "Total Items: " + Item.Count.ToString("D2");
    private static string ActiveSticks => "Active Items: " + Level.itemCount.ToString("D2");
}


public static class InfoPanel
{
    private static GameObject infoCanvas;
    private static bool ShowInfo
    {
        set
        {
            if (value)
                infoCanvas = "Debug/InfoPanel_Canvas".ResourceInst(SceneLocator.UI);
            else
                if(infoCanvas)
                    Object.Destroy(infoCanvas);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RuntimeInitialze()
    {
        /*
        if(SceneManager.GetActiveScene().name == "main")
            BoolSwitch.Link("Info/Info Panel", false, v=> ShowInfo = v);
            */
    }
}