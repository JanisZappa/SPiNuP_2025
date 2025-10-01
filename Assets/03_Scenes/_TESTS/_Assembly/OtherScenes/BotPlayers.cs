using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public static class BotPlayers
{
    private static string[] PlayerNames;

    private static Dictionary<string, int> nameIDs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreatePlayerNames()
    {
        PlayerNames = (Resources.Load("BotNames") as TextAsset).text.Split('\n');
        
        nameIDs = new Dictionary<string, int>();
        for (int i = 0; i < PlayerNames.Length; i++)
        {
            PlayerNames[i] = Regex.Replace(PlayerNames[i], @"\t|\n|\r", "");
            nameIDs.Add(PlayerNames[i], i + 1);
        }
    }

    private static Text botText;
    public static void GetName(ref string name, ref int id)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int r = Random.Range(0, PlayerNames.Length);
        id = r + 1;
        name = PlayerNames[r];

        if (botText != null)
            botText.text = PlayerNames[r];
    }


    public static string RandomName
    {
        get { return PlayerNames[Random.Range(0, PlayerNames.Length)]; }
    }


    public static string GetName(int id)
    {
        return PlayerNames[id];
    }


    public static int GetNameID(string name)
    {
        return nameIDs[name];
    }
}
