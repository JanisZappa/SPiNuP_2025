using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public static class SetVolume
{
    private static AudioMixer everything;
    
    public static readonly BoolSwitch Mute =
        new("Audio/Mute", false, (g)=> UpdateVolume(g));

   
    private static void UpdateVolume(bool value)
    {
        if(!everything)
            everything = Resources.Load("Audio/Everything") as AudioMixer;
        
        everything.SetFloat("masterVolume", value || GTime.Paused? -80 : 0);
    }

    public static void UpdateVolume()
    {
        UpdateVolume(Mute);
    }

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void MainFirstStart()
    {
        Run.PutInScene();
        Run.Inst.StartCoroutine(WaitRun());
    }


    private static IEnumerator WaitRun()
    {
        yield return null;
        
        if(SceneManager.GetActiveScene().name == "main")
            UpdateVolume(Mute);
    }
}
