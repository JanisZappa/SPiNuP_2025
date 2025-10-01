using UnityEngine;


public class BlurCam : MonoBehaviour 
{
    public Camera backCam;
    
    [Space(10)] 
    
    public Material blurMat;

    private RenderTexture renderTex;
    private static readonly BoolSwitch blurBackground = new("Camera/Blur", true);
    
    
    private void OnPreRender()
    {
        if (blurBackground)
        {
            renderTex             = RenderTexture.GetTemporary((int)(ScreenControll.Width * 1), (int)(ScreenControll.Height * 1), 8);
            renderTex.filterMode  = FilterMode.Trilinear;
            backCam.targetTexture = renderTex;
        }
        else
            if(backCam.targetTexture)
                backCam.targetTexture = null;
    }
    
    
    private void OnPostRender()
    {
        if(!blurBackground)
            return;
        
        backCam.targetTexture = null;
        Graphics.Blit(renderTex, null, blurMat);
        RenderTexture.ReleaseTemporary(renderTex);
    }
}
