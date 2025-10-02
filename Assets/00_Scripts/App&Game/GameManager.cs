using System.Collections;
using GameModeStuff;
using UnityEngine;


namespace GameModeStuff
{
    public enum Mode : byte { Select, SpinUp, Creator }//, ColorSticks }
    
    public enum GameState { None, ServerConnect, GameLoad, GameStart, GameOver, GameWon }
    
    public interface IGameMode
    {
        void Load();
        void Restart();        
        void StateUpdate();
        IEnumerator GameOver();
    }
}


public class GameManager : Singleton<GameManager>
{
    public static GameState State;

    public static Mode SavedMode
    {
        get => Mode.SpinUp;//(Mode) (Application.isEditor? PlayerPrefs.GetInt("GameMode") : 1);
        set => PlayerPrefs.SetInt("GameMode", (int)value);
    }
    
    private static Mode currentMode;
    public static bool Running;
    public static bool IsCreator => currentMode == Mode.Creator;

    private static IGameMode mode;
    private static readonly IGameMode[] modes = { null, new Mode_SpinUp(), new Creator(), null };
    
    private static GameCam gameCam;
    
    public delegate void GameStart();
    public static event GameStart OnGameStart;
    
    
    private void Start()
    {
        ChangeState(GameState.ServerConnect);
    }

    
    private void Update()
    {
        Controll.Update();
        
        if (!Running) return;
        
        GTime.TimeStep();
        mode.StateUpdate();

        ByteReplays.GameUpdate();
        Spinner.AllStateUpdate();
        Collector.GameUpdate();
        gameCam.CameraUpdate();
        Sound.SoundUpdate();
        Level.Refresh();
        ActorAnimator.LatePoseSet();
        
        if(SaveMeshFrameRange.Saving)
            SaveMeshFrameRange.WriteToFile();
    }
    
    
    public static void ChangeState(GameState state)
    {
        State = state;
        
        Debug.Log("Change State " + state);

        switch (state)
        {
            case GameState.ServerConnect:
                Inst.StartCoroutine(Database.InitServerConnection());
                break;
            
            case GameState.GameLoad:
                LoadGameMode(SavedMode);
                break;
            
            case GameState.GameStart:
                Level.GameStart();
                gameCam.ResetCamera();
                ActorAnimator.GameStart(GTime.StartNewGame_GetTime());  
                OnGameStart?.Invoke();
                GarbageMachine.Collect();
                
                mode.Restart();
                Running = true;
                break;
            
            case GameState.GameOver:
                Running = false;
                Inst.StartCoroutine(GameEnd());
                break;
            
            case GameState.GameWon:
                Inst.StartCoroutine(DelayedGameEnd());
                break;
        }
    }

    
    public static void LoadGameMode(Mode newMode)
    {
        currentMode     = newMode;
        mode = modes[(int)currentMode];
        if (mode == null)
        {
            UI_Manager.ShowUI(UIType.MainMenu, true, true);    
            return;
        }

        Inst.StartCoroutine(GameLoad());
    }

    
    private static IEnumerator GameLoad()
    {
        yield return ServerTime.GetTime();
        gameCam = CamManager.GameLoad();
        yield return Level.GameLoad();
        Spinner.GameLoad();
        Sound.GameLoad();
        
        mode.Load();
        
        ChangeState(GameState.GameStart);
    }

    
    private static IEnumerator GameEnd()
    {
        yield return mode.GameOver();
        
        while(Spinner.active.Count > 0)
            Spinner.active[0].Disable();
        
        ChangeState(GameState.GameStart);
    }


    private static IEnumerator DelayedGameEnd()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            GTime.Speed = Mathf.SmoothStep(1, .5f, t);
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        GTime.Speed = 1;
        Running = false;
        yield return GameEnd();
    }
    
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!Running || !Application.isMobilePlatform)
            return;

        if (!GTime.Paused)
            GTime.SetPause(true);
        else
            TimeSliderUI.playingSlowMo = false;
    }
}