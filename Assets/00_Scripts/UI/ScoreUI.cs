using System.Collections;
using TMPro;
using UnityEngine;


public class ScoreUI : MonoBehaviour 
{
    public  TextMeshProUGUI scoreText, highScoreText;
    private static GameObject scoreObject, highscoreObject;
    private RectTransform scoreRect, highScoreRect;

    private BoolSwitch showScore;

    
    private void Awake()
    {
        scoreObject     = scoreText.gameObject;
        highscoreObject = highScoreText.transform.parent.gameObject;
        
        showScore = new("UI/Score", true, b =>
        {
            scoreObject.SetActive(b);
            highscoreObject.SetActive(b);
        });

        scoreRect     = scoreText.rectTransform;
        highScoreRect = highscoreObject.GetComponent<RectTransform>();
    }


    private void OnEnable()
    {
        //BoolSwitch.Link("UI/Score", true, v => showScore = v);
        Score.SetScoreUI(this);
    }

    
    public void SetScore(int score)
    {
        if (score == 0)
            scoreText.text = "X";
        else
        {
            scoreText.text = score.PrepString();

            /*if (score > oldScore && score > 0)
                StartCoroutine(Pop());

            if (score < oldScore)
                StartCoroutine(UnPop());*/
        }
    }

    
    public void SetHighScore(int highScore, bool animate)
    {
        highScoreText.text = highScore.PrepString();

        /*if (animate)
            StartCoroutine(PopHighScore());*/
    }

    
    private IEnumerator Pop()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            lerp += Time.deltaTime * 10;
            float scale = Mathf.Lerp(1.45f, 1, lerp);
            scoreRect.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    
    private IEnumerator UnPop()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            float scale = Mathf.Lerp(.5f, 1, lerp);
            scoreRect.localScale = new Vector3(scale, scale, scale);
            lerp += Time.deltaTime * 10;
            yield return null;
        }
    }

    
    private IEnumerator PopHighScore()
    {
        for (int i = 0; i < 6; i++)
        {
            float count = 0;
            while (count < 1)
            {
                float scale = i % 2 == 0 ? Mathf.Lerp(1, 1.2f, count) : Mathf.Lerp(1.2f, 1, count);
                highScoreRect.localScale = new Vector3(scale, scale, scale);
                count += Time.deltaTime * 15;
                yield return null;
            }
        }
    }
}
