using UnityEngine;


public class SceneLocator : MonoBehaviour
{
    public Transform ui, characters, items, fluff, collectables, level, cam, sound;

    private void Awake()
    {
        if (Application.isEditor)
        {
            UI           = ui;
            Characters   = characters;
            Items        = items;
            Fluff        = fluff;
            Collectables = collectables;
            Level        = level;
            Sound        = sound;
        }
        
        Cam = cam;
    }

    public static Transform UI, Characters, Items, Fluff, Collectables, Level, Cam, Sound;
}
