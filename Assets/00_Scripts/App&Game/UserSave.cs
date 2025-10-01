using UnityEngine;


public static class UserSave
{
    public static int ID
    {
        get { return PlayerPrefs.GetInt("UserSave_Account_ID");}
        set {        PlayerPrefs.SetInt("UserSave_Account_ID", value);}
    }

    
    public static string Name
    {
        get { return PlayerPrefs.GetString("UserSave_Name");}
        set {        PlayerPrefs.SetString("UserSave_Name", value);}
    }
    

    public static int Highscore
    {
        get { return PlayerPrefs.GetInt("UserSave_Highscore");}
        set {        PlayerPrefs.SetInt("UserSave_Highscore", value);}
    }
    
    public static float Height
    {
        get { return PlayerPrefs.GetFloat("UserSave_Height");}
        set {        PlayerPrefs.SetFloat("UserSave_Height", value); }
    }
            
    public static int Coins
    {
        get { return PlayerPrefs.GetInt("UserSave_Coins");}
        set {        PlayerPrefs.SetInt("UserSave_Coins", value);}
    }
    
    public static int Level
    {
        get { return PlayerPrefs.GetInt("UserSave_Level");}
        set {        PlayerPrefs.SetInt("UserSave_Level", value);}
    }
   
    public static float PlayTime
    {
        get { return PlayerPrefs.GetFloat("UserSave_PlayTime");}
        set {        PlayerPrefs.SetFloat("UserSave_PlayTime", value); }
    }
    
    
    public static byte ColorValue
    {
        get { return (byte) PlayerPrefs.GetInt("UserSave_ColorValue"); }
        set { PlayerPrefs.SetInt("UserSave_ColorValue", value); }
    }
}
