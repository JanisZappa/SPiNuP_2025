using UnityEngine;


public class Wireframe : MonoBehaviour
{
    private static readonly BoolSwitch active = new("Visuals/Wireframe", false);
    
    public static bool Active => active && !Application.isMobilePlatform;


    private void OnPreRender()
    {
        GL.wireframe = Active;
    }

    
    private void OnPostRender()
    {
        GL.wireframe = false;
    }
}