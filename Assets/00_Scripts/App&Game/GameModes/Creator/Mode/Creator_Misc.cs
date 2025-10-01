using LevelElements;
using UnityEngine;


public partial class Creator
{
    private static elementType currentElementType = 0;
    public static ElementMask currentFilter = null;

    private static bool TappedUI;

    
    public static void SetElementType(elementType elementType)
    {
        currentElementType = elementType;
        TappedUI           = true;
    }
    
    
    private static void SetStartStick()
    {
        if (Input.GetKeyDown(KeyCode.Home))
            Level.StartStick = (Item)ElementEdit.element;
    }


    public static void NewLevel()
    {
        Level.ClearLevel();
    }


    private static void CenterCam()
    {
        if (Input.GetKeyDown(KeyCode.C) && !Input.GetKey(KeyCode.LeftControl))
            MoveCam.pos = ElementEdit.element.GetPos(GTime.Now);
    }


    private static void Saving()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            UI_Creator.ShowSavePrompt(LevelSaveLoad.CurrentLevel);
    }
}
