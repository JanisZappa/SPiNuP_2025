using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class ServerTime : Singleton<ServerTime>
{
    private static StringDisplay display;
    private static TimeSpan      difference;
    private static DateTime      levelStart;
    private static int           webMilliseconds;
    private static bool          showTime;

    private static readonly BoolSwitch showLevelLiveTime = new("Info/Server Time", false, b =>
    {
        display = showTime ? UI_Manager.ShowUI(UIType.Clock, b).GetComponent<StringDisplay>() : null;
    });
    

    public static IEnumerator GetTime()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            DateTime same = DateTime.Now;
            GotDifference(same, same);
            yield break;
        }


        float timeNow = Time.realtimeSinceStartup;

        UnityWebRequest www = UnityWebRequest.Get(Database.ServerTimePHP);
        www.SendWebRequest();
        
        
        
        float waitTime = 0;
        while (!www.isDone)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= 1)
                yield break;

            yield return null;
        }

        if (!www.isDone || www.downloadHandler.text.Length == 0)
            yield break;


        webMilliseconds = Mathf.RoundToInt((Time.realtimeSinceStartup - timeNow) * 1000);

        DateTime localNow = DateTime.Now;

        string firstPart  = www.downloadHandler.text.Substring(0, 19);
        string secondPart = www.downloadHandler.text.Replace(firstPart + "-", "");

        bool gotServerTime   = DateTime.TryParseExact(firstPart, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var serverNow);
        bool gotMilliseconds = int.TryParse(secondPart, out var milliseconds);

        if (!gotServerTime || !gotMilliseconds)
            yield break;

        serverNow = serverNow.AddMilliseconds(webMilliseconds + milliseconds);
        
        GotDifference(localNow, serverNow);
        
        www.Dispose();
    }

    private static void GotDifference(DateTime local, DateTime server)
    {
        difference = local - server;
        
        int hours = server.Hour % 8;
        levelStart = new DateTime().AddHours(server.Hour - hours);
    }
    
    private readonly string[]        displayStrings = new string[3];
    private readonly StringBuilder[] builders       = new StringBuilder[3];

    
    private void Awake()
    {
        for (int i = 0; i < 3; i++)
            builders[i] = new StringBuilder(30, 30);
    }
    
    
    private void Update()
    {
        if (!showLevelLiveTime)
            return;

        for (int i = 0; i < 3; i++)
            builders[i].Length = 0;
        
        
        builders[0].Append(webMilliseconds);
        
        DateTime serverNow = DateTime.Now - difference;
        builders[1].Append(serverNow.Hour.ToString("D2")).Append(":").
                    Append(serverNow.Minute.ToString("D2")).Append(":").
                    Append(serverNow.Second.ToString("D2"));
        
        TimeSpan levelDuration = serverNow - levelStart;
        builders[2].Append(levelDuration.Hours.ToString("D2")).Append(":").
                    Append(levelDuration.Minutes.ToString("D2")).Append(":").
                    Append(levelDuration.Seconds.ToString("D2"));

        for (int i = 0; i < 3; i++)
            displayStrings[i] = builders[i].ToString();
        
        display.SetTexts(displayStrings);
    }


    public static float GetGameStartTime()
    {
        return (float)((DateTime.Now - difference - levelStart).TotalSeconds % GTime.LoopTime);
    }
}

