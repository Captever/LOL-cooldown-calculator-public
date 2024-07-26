using System.Collections;
using TMPro;
using UnityEngine;


public class GameManager : SingletonObj<GameManager>
{
    public enum GameState
    {
        None = -1,
        Preloading = 0,
        Info = 1,
        OnGame = 2
    }
    public enum NetworkProcess
    {
        None = -1,
        GetVersion,
        GetSummoner,
        GetSpectator,
        GetChampionsData,
        GetSummonerSpellsData,
        OnUsing_Undefined = 100
    }
    public GameState GAME_STATE;
    public NetworkProcess NETWORK_PROCESS;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    private void Init()
    {
        GAME_STATE = GameState.None;
        NETWORK_PROCESS = NetworkProcess.None;
    }
}