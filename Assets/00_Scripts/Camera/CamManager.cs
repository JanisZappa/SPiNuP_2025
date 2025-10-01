using UnityEngine;


public static class CamManager
{
    private static GameCam    gameCam;
    private static GameObject camInst;
    private static GameObject menuCam;

    public static GameCam GameLoad()
    {
        menuCam = SceneLocator.Cam.GetChild(0).gameObject;
        menuCam.SetActive(false);
        
        camInst = "Camera/GameCam".ResourceInst(SceneLocator.Cam);
        gameCam = camInst.GetComponent<GameCam>();

        DRAW.DrawCam = gameCam.cam;
        
        return gameCam;
    }
}