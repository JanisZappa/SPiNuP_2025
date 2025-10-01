using System.Collections;
using TMPro;
using UnityEngine;


public class TableLine : MonoBehaviour {

    public TextMeshProUGUI pos, playerName, score;
    
    [Space(5)]
    public Color[] blinkColors;

  
	public void SetLine(string rank, string name, int score)
    {
        pos.text = rank;

        const string empty = "...";
        bool isEmpty = score == 0 || name == "";
        playerName.text = isEmpty? empty : name;
        this.score.text = isEmpty? empty : score.ToString();

        Color setColor = name == UserSave.Name? blinkColors[0] : new Color(1, 1, 1, 1);
        pos.color = playerName.color = this.score.color = setColor;

        if (name == UserSave.Name && score == UserSave.Highscore)
            StartCoroutine(Blink());
    }

    
    private readonly WaitForSeconds wait = new WaitForSeconds(.15f);
    private IEnumerator Blink()
    {
        int colorPick = 0;
        while(Leaderboard.visible)
        {
            colorPick = (colorPick + 1) % blinkColors.Length;
                   pos.color = blinkColors[colorPick];
            playerName.color = blinkColors[colorPick];
                 score.color = blinkColors[colorPick];
            
            yield return wait;
        }
    }
}
