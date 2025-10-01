using System.Collections;
using Anim;
using GameModeStuff;


public class Mode_SpinUp : IGameMode
{
    public void Load()
    {
        //ClubMaster.GameLoad();
        Leaderboard.GetBoard();
        UI_Manager.ShowUI(UIType.SpinUp, true, true);
    }

    
    public void Restart()
    {
        Spinner.Get(0).Enable(BotPlayers.RandomName, new Costume());
                
         Score.ResetScore();
        Height.ResetHeight();
          Mood.Reset();
    }
   
    
    public void StateUpdate()
    {
        //ClubMaster.ClubUpdate();
        
        Mood.Update();
    }

    
    public IEnumerator GameOver()
    {   
        Run.Inst.StartCoroutine(ReplaySaveLoad.SaveReplay());
        Run.Inst.StartCoroutine(Score.CheckForHighScore());
        
        yield return null;
    }

    
    public IEnumerator HouseKeeping()
    {
    //  Get the Player Highscore  //
        
     
    //  Check if Game has been running  //
    //  Upload Replay if Highscore  is better  //
    
    
    //  Download enough replays  //

        yield break;
    
    //yield return DownloadReplaysLikeCrazy();
    }
}
