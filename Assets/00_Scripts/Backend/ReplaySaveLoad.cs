using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;


public static class ReplaySaveLoad 
{
    public  static string IDHex     => HexString + HexString;
    private static string HexString => Random.Range(0, 65535).ToString("X4");

    private static readonly BoolSwitch SaveLocally = new ("Tape/Save Local Replay", false);
    
    private const string LocalReplayFolder = @"D:\_SYNC HARDER\SPINNER\LocalReplays\";
    
    
    public static IEnumerator SaveReplay()
    {
        Spinner spinner = Spinner.GetPlayer;
        
        string name  = spinner.name;
        byte[] bytes = spinner.GetReplayBytes();

        if (bytes.Length == 0)
        {
            Debug.Log("Replay wasn't Serializable ...".B_Yellow());
            yield break;
        }

        if (Application.isEditor && SaveLocally)
            SaveLocalReplay(bytes);

        if (!Database.IsConnected)
            yield break;

        Debug.Log("Uploading Replay for " + name);

        {
            WWWForm form = new WWWForm();
            form.AddField("i", BotPlayers.GetNameID(name));
            form.AddField("s", Score.PlayerOneScore);
            form.AddBinaryData("b", bytes);

            UnityWebRequest www = UnityWebRequest.Post(Database.UploadScorePHP, form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error.B_Red());

            www.Dispose();
        }
    }
    
    
    public static IEnumerator GetReplayBytes(Action<byte[]> byteCallback)
    {
        if (!Database.IsConnected && !Application.isMobilePlatform)
        {
            byteCallback(GetLocalReplay());
            yield break;
        }
        
        UnityWebRequest www = UnityWebRequest.Get(Database.GetRandomReplayPHP);
        yield return www.SendWebRequest();
        
        if(!www.isNetworkError && !www.isHttpError)
            byteCallback(www.downloadHandler.data);
        
        www.Dispose();
    }


    private static void SaveLocalReplay(byte[] bytes)
    {
        string path = LocalReplayFolder + FancyString.GetRandom(10) + ".rpl";
        
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            fs.Write(bytes, 0, bytes.Length);
    }


    private static byte[] GetLocalReplay()
    {
        string[] files = Directory.GetFiles(LocalReplayFolder);
        
        string path = files[Random.Range(0, files.Length)];

        return File.ReadAllBytes(path);
    }
}
