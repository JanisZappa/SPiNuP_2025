using System.Collections.Generic;
using UnityEngine;


public enum Key
{
    P1Action,
    P2Action,
        
    Rewind,
    Pause,
    Unpause,
    
    Dev_SquashTest,
    Dev_LightToggle,
    Dev_CostumeColor,
    Dev_SideSwitch,
    Dev_CamModeToggle,
    Dev_ColorShift
}


public static class KeyMap
{
    static KeyMap()
    {
        keyDict = new Dictionary<Key, KeyCode[]>(new Key_Comparer())
        {
            { Key.P1Action,          new []{ KeyCode.Space, 
                                             KeyCode.Mouse0, 
                                             KeyCode.KeypadEnter, 
                                             KeyCode.Joystick1Button0,
                                             KeyCode.Joystick1Button1}},
            { Key.P2Action,          new []{ KeyCode.Alpha8 }},
            { Key.Rewind,            new []{ KeyCode.KeypadPeriod, 
                                             KeyCode.Mouse1, 
                                             KeyCode.Joystick1Button2, 
                                             KeyCode.Delete, 
                                             KeyCode.Minus}},
            { Key.Pause,             new []{ KeyCode.Mouse2, 
                                             KeyCode.Joystick1Button7,
                                             KeyCode.Pause }},
            { Key.Unpause,           new []{ KeyCode.Mouse1, 
                                             KeyCode.Joystick1Button7, 
                                             KeyCode.Pause }},
            { Key.Dev_SquashTest,    new []{ KeyCode.K }},
            { Key.Dev_LightToggle,   new []{ KeyCode.F8, 
                                             KeyCode.Joystick1Button6 }},
            { Key.Dev_CostumeColor,  new []{ KeyCode.L, 
                                             KeyCode.Joystick1Button3 }},
            { Key.Dev_CamModeToggle, new []{ KeyCode.LeftControl, 
                                             KeyCode.Joystick1Button4 }},
            { Key.Dev_SideSwitch,    new []{ KeyCode.Alpha0, 
                                             KeyCode.Keypad1,
                                             KeyCode.Joystick1Button5 }},
            { Key.Dev_ColorShift,    new []{ KeyCode.C, 
                                             KeyCode.Joystick1Button8 }}
        };
        
        List<KeyCode> getKeys = new List<KeyCode>();
        foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            getKeys.Add(vKey);

        keyArray = getKeys.ToArray();
    }
    
    private static KeyCode[] keyArray;
    
    private struct Key_Comparer : IEqualityComparer<Key>
    {
        public bool Equals(Key x, Key y) { return x == y;   }
        public int  GetHashCode(Key obj) { return (int)obj; }
    }

    
    private static readonly Dictionary<Key, KeyCode[]> keyDict;
    
    
    public static bool AnyInput
    {
        get { return Down(Key.P1Action) || 
                     Down(Key.P2Action); }
    }
    
    
    public static bool CheckInput()
    {
        return !UI_Manager.HitUI && 
               !GTime.Paused && 
               !ShakeRewind.Rewinding && 
               Down(Key.P1Action);
    }


    public static bool Down(Key key)
    {
        KeyCode[] keys = keyDict[key];
        for(int e = 0; e < keys.Length; e++)
            if(Input.GetKeyDown(keys[e]))
                return true;

        return false;
    }
        
        
    public static bool Hold(Key key)
    {
        KeyCode[] keys = keyDict[key];
        for(int e = 0; e < keys.Length; e++)
            if (Input.GetKey(keys[e]))
                return true;

        return false;
    }
        
        
    public static bool Up(Key key)
    {
        KeyCode[] keys = keyDict[key];
        for(int e = 0; e < keys.Length; e++)
            if(Input.GetKeyUp(keys[e]))
                return true;

        return false;
    }

    
    private const KeyCode Null = KeyCode.Joystick8Button19;
    public static bool Down(KeyCode keyA, KeyCode keyB = Null, KeyCode keyC = Null, KeyCode keyD = Null)
    {
        if (Input.GetKeyDown(keyA))
            return true;
        
        if (keyB != Null && Input.GetKeyDown(keyB))
            return true;
        
        if (keyC != Null && Input.GetKeyDown(keyC))
            return true;
        
        return keyD != Null && Input.GetKeyDown(keyD);
    }
    
    
    public static bool Hold(KeyCode keyA, KeyCode keyB = Null, KeyCode keyC = Null, KeyCode keyD = Null)
    {
        if (Input.GetKey(keyA))
            return true;
        
        if (keyB != Null && Input.GetKey(keyB))
            return true;
        
        if (keyC != Null && Input.GetKey(keyC))
            return true;
        
        return keyD != Null && Input.GetKey(keyD);
    }
    
    
    public static bool Up(KeyCode keyA, KeyCode keyB = Null, KeyCode keyC = Null, KeyCode keyD = Null)
    {
        if (Input.GetKeyUp(keyA))
            return true;
        
        if (keyB != Null && Input.GetKeyUp(keyB))
            return true;
        
        if (keyC != Null && Input.GetKeyUp(keyC))
            return true;
        
        return keyD != Null && Input.GetKeyUp(keyD);
    }
    
    
    public static KeyCode GetDownKey
    {
        get
        {
            int length = keyArray.Length;
            for (int i = 0; i < length; i++)
            {
                KeyCode code = keyArray[i];
                
                if (Input.GetKeyDown(code))
                    return code;
            }
           
            return KeyCode.None;
        }
    }
}

