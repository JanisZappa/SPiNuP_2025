using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;


public class Leaderboard : Singleton<Leaderboard> 
{
    public static readonly BoolSwitch showLeaderBoard = new("UI/Leader Board", false);

    public GameObject     panel, tableLine;
    public Transform      lineHolder;
    public AnimationCurve tableLerp;
    
    private RectTransform myRect;
    private Vector3       endPos, startPos, gonePos;
    private GameObject[]  lines;
    
    public static bool    visible;
    
    
	private void Awake () 
    {
        myRect = GetComponent<RectTransform>();
        endPos = myRect.anchoredPosition3D;

        Vector3 upVector = Quaternion.AngleAxis(myRect.localEulerAngles.z, transform.forward) * V3.right;
        startPos = endPos + upVector * 1400;
        gonePos  = endPos + upVector * -1400;
        
        lines = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            GameObject newLine = Instantiate(tableLine, lineHolder, false);
            RectTransform lineRect = newLine.GetComponent<RectTransform>();
            lineRect.anchoredPosition3D = new Vector3(0, i * -60, 0);
            lineRect.localEulerAngles = Vector3.zero;
           
            lines[i] = newLine;
            lines[i].GetComponent<TableLine>().SetLine((i + 1).ToString(), "", 0);
        }

        myRect.anchoredPosition3D = startPos;
	}


    private void OnEnable()
    {
        panel.SetActive(false);
    }


    private static void SetLine(int nr, string name, int score)
    {
        Inst.lines[nr].GetComponent<TableLine>().SetLine((nr+1).ToString(), name, score);
    }


    private void ShowHideTable(bool show)
    {
        if(visible != show)
        {
            visible = show;
            
            StartCoroutine(SlideInOut(show));
        }
    }


    private IEnumerator SlideInOut(bool comeIn)
    {
        panel.SetActive(true);
        
       float lerp = 0;

       Vector3 startP = (comeIn)? startPos : endPos;
       Vector3 endP = (comeIn) ? endPos : gonePos;

        while(lerp<1)
        {
            lerp += Time.deltaTime * 3;
            myRect.anchoredPosition3D = Vector3.Lerp(startP, endP, tableLerp.Evaluate(lerp));
            yield return null;
        }
        
        panel.SetActive(visible);
    }

    
    /*private void Update()
    {
        if(visible && KeyMap.AnyInput)
            ShowHideTable(false);
    }*/
    
    
    public static void GetBoard()
    {
        if (showLeaderBoard)
            Inst.StartCoroutine(Inst.GetOnlineLeaderboard());
    }


    public IEnumerator GetOnlineLeaderboard(int start = 0)
    {
        WWWForm form = new WWWForm();
                form.AddField("o", start);
                form.AddField("l", 10);
        
        UnityWebRequest www = UnityWebRequest.Post(Database.GetBoardPHP, form);
        yield return www.SendWebRequest();
                
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Couldn't download Leaderboard".B_Red());
        }
        else
        {
            JSONNode jsonNode = JSON.Parse(www.downloadHandler.text);
        
            for (int i = 0; i < jsonNode.Count; i++)
                SetLine(i, jsonNode[i][0].ToString().Replace("\"",""), int.Parse(jsonNode[i][1].ToString().Replace("\"","")));
        
            Inst.ShowHideTable(true);
        }
        
        www.Dispose();
    }
}
