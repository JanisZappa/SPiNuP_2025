using System.Collections;
using Clips;
using LevelElements;
using UnityEngine;


public static class Score
{
    static Score()
    {
        playerScores = new int[Spinner.Count];
        scoredSticks = new int[Item.TotalCount];
    }
    
    private static readonly int[] playerScores, scoredSticks;
    
    public  static int PlayerOneScore { get { return playerScores[0]; } }
    private static int HighScore;
    private static int gameID;

    private static ScoreUI _scoreUI;

    
    public static void SetScoreUI(ScoreUI scoreUI)
    {
        _scoreUI = scoreUI;
    }

    
    public static void DeleteHighscore()
    {
        UserSave.Height = 0;
        HighScore = UserSave.Highscore;

        if(_scoreUI != null)
            _scoreUI.SetHighScore(HighScore, false);
    }

    
    public static void ResetScore()
    {
        for (int i = 0; i < playerScores.Length; i++)
            playerScores[i] = 0;

        gameID += Spinner.Count;
        HighScore = UserSave.Highscore;

        if ( _scoreUI != null )
            _scoreUI.SetHighScore(HighScore, false);

        if ( _scoreUI != null )
            _scoreUI.SetScore(PlayerOneScore);
    }

    
    public static void OnSwing(Swing swing)
    {
        Item item = swing.startStick.Item;
       
        if (item.elementType != elementType.Stick || GameManager.IsCreator || !swing.spinner.isPlayer)
            return;
        
    //  Update ScoreSticks  //
        ScoreStick.AddNewState(swing);

        if (item == Level.StartStick)
            return;

        int spinnerID   = swing.spinner.ID;
        int checkNumber = spinnerID + gameID;
        if (scoredSticks[item.ID] == checkNumber)
        {
            Sound.Get(Audio.UI.OldStick).Play();
            return;
        }
        
        playerScores[spinnerID]++;
        scoredSticks[item.ID] = checkNumber;
        
        Sound.Get(Audio.UI.ScoreUp).Play();

        if ( _scoreUI != null )
            _scoreUI.SetScore(PlayerOneScore);
    }
    
    
    public static void OnlineScore(int onlineScore)
    {
        if(onlineScore > HighScore)
        {
            Debug.Log("Better Online Score".B_Yellow());
            
            UserSave.Highscore = HighScore = onlineScore;

            if ( _scoreUI != null )
                _scoreUI.SetHighScore(HighScore, false);
        }
    }

    
    public static IEnumerator CheckForHighScore()
    {
        if ( PlayerOneScore > HighScore )
        {
            if ( Database.IsConnected )
                UserSave.Highscore = PlayerOneScore;

            HighScore = PlayerOneScore;

            if ( _scoreUI != null )
                _scoreUI.SetHighScore(HighScore, true);

            Sound.Get(Audio.UI.HighScore).Play();

            Debug.Log("New Highscore! ".B_Purple() + Emoji.Success.B_Teal());

            // yield return ReplaySaveLoad.UploadReplay();
            // yield return OnlineTable.GetBoard(0);
            yield break;
        }
    }
}

