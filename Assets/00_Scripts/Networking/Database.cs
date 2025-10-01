using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using GameModeStuff;

public static class Database
{
    private static readonly BoolSwitch SkipServerStuff = new("Dev/Skip Server Stuff", false);
    
    public static bool IsConnected;
    
    public static int ServerVersion, 
                      PlayerScore, 
                      PlayerRank;

    private const int GameVersion = 3;


    public static IEnumerator InitServerConnection()
    {
        IsConnected = false;
        
        if (SkipServerStuff)
        {
            Debug.Log("Skipping Server Stuff".B_Purple());
            GameManager.ChangeState(GameState.GameLoad);
            yield break;  
        }
        
        yield return GetServerVersion();

        if (!IsConnected)
        {
        //  Let's play anyway for now ;)  //
            GameManager.ChangeState(GameState.GameLoad);
            yield break;
        }
            

        if (ServerVersion != GameVersion)
        {
            Debug.Log("Wrong Version");
            yield break;
        }

        string checkName = BotPlayers.RandomName;

        yield return GetPlayerScore(checkName);
        yield return GetPlayerRank(checkName);
        yield return GetDay();
        yield return GetServerTime();
        //yield return GetCombineReplays();
        
        GameManager.ChangeState(GameState.GameLoad);
    }


    public static IEnumerator GetServerVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetServerVersionPHP);
        yield return www.SendWebRequest();
                
        if(www.isNetworkError || www.isHttpError)
        {
            IsConnected = false;
            Debug.Log("Connection to Server failed".B_Red());
            Debug.LogFormat("{0}".B_Red(), www.error);
        }
        else
        {
            ServerVersion = int.Parse(www.downloadHandler.text);
            Debug.Log("Server Version: " + ServerVersion);
            IsConnected = true;
        }
        
        www.Dispose();
    }
    
    
    public static IEnumerator GetPlayerScore(string name)
    {
        WWWForm form = new WWWForm();
        form.AddField("i", BotPlayers.GetNameID(name));
        
        UnityWebRequest www = UnityWebRequest.Post(GetPlayerScorePHP, form);
        yield return www.SendWebRequest();
                
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("Couldn't get {0} Score".B_Red(), name);
        }
        else
        {
            Debug.Log(name + " -> Score -> " + www.downloadHandler.text);
            
            //PlayerScore = int.Parse(www.downloadHandler.text);
            //Debug.LogFormat("{0} Score: {1}", name,  PlayerScore);
        }
        
        www.Dispose();
    }
    
    
    public static IEnumerator GetPlayerRank(string name)
    {
        WWWForm form = new WWWForm();
        form.AddField("i", BotPlayers.GetNameID(name));
        
        UnityWebRequest www = UnityWebRequest.Post(GetPlayerRankPHP, form);
        yield return www.SendWebRequest();
                
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("Couldn't get {0} Rank".B_Red(), name);
        }
        else
        {
            Debug.Log(name + " -> Rank -> " + www.downloadHandler.text);
            
            //PlayerRank = int.Parse(www.downloadHandler.text);
            //Debug.LogFormat("{0} Rank: {1}", name, PlayerRank);
        }
        
        www.Dispose();
    }


    public static IEnumerator GetScoreCount()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetScoreCountPHP);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Couldn't download ScoreCount".B_Red());
        }
        else
        {
            Debug.Log("Database Score Count: " + www.downloadHandler.text);
        }
        
        www.Dispose();
    }
    
    
    public static IEnumerator GetCombineReplays()
    {
        UnityWebRequest www = UnityWebRequest.Get(CombinedReplaysPHP);
        yield return www.SendWebRequest();
                
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("Couldn't get combine Replays".B_Red());
        }
        else
        {
            byte[] returnBytes = www.downloadHandler.data;
            int length = returnBytes.Length;

            ByteReplays.ResetCount();
            
            
            ByteStream.Set(returnBytes);
            while (ByteStream.stream.Position < length)
                if (!ByteReplays.AddReplay())
                    break;

            if (ByteReplays.Count > 0)
                Debug.LogFormat("Got {0} Replays - Size: {1:F1} kb", ByteReplays.Count, length / 1024f);   
        }
        
        www.Dispose();
    }
    

    public static IEnumerator GetDay()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetDayPHP);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Couldn't get Day".B_Red());
        }
        else
        {
            Debug.Log("Day: " + www.downloadHandler.text);
        }
        
        www.Dispose();
    }
    
    
    public static IEnumerator GetServerTime()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerTimePHP);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log("Couldn't get Servertime".B_Red());
        else
            Debug.Log("Servertime: " + www.downloadHandler.text);
        
        www.Dispose();
    }
    
    
    

//  PHP Adresses  //
    private const string 
        Server = "https://checkandiout.com/_Servers/SPiNuP/spinnerdb/",        
          Info = Server + "info/",            
        Player = Server + "player/";


    public const string 
        GetServerVersionPHP = Info   + "getServerVersion.php",
                  GetDayPHP = Info   + "getDay.php",
              ServerTimePHP = Info   + "servertime.php",
                GetBoardPHP = Info   + "getTablejSON.php",
        
          GetPlayerScorePHP = Player + "getPlayerScore.php",
           GetPlayerRankPHP = Player + "getPlayerRank.php",
        
                   LoginPHP = Server + "login.php",
         CombinedReplaysPHP = Server + "combineReplays.php",
           GetScoreCountPHP = Server + "getScoreCount.php",
         GetRandomReplayPHP = Server + "getRandomReplay.php",
             UploadScorePHP = Server + "uploadScore.php";
}
