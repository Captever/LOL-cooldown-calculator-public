using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoManager : EachSceneManager
{
    [SerializeField] private TMP_InputField summonerNameInput = null;
    [SerializeField] private GameObject errorPanel = null;
    [SerializeField] private TextMeshProUGUI platformLabel = null;

    // Start is called before the first frame update
    void Start()
    {
        InfoInit();
    }


    // initialize scene
    private void InfoInit()
    {
        if (GameManager.Instance.GAME_STATE == GameManager.GameState.Info) return;  // To prevent duplicate

        GameManager.Instance.GAME_STATE = GameManager.GameState.Info;

        // initialize textField
        summonerNameInput.text = "";
        ErrorMessageInit();
    }
    // To initialize error message panel
    private void ErrorMessageInit()
    {
        errorPanel.SetActive(false);
    }
    
    // To confirm summoner information when press 'Enter' button
    public void ConfirmingSummoner()
    {
        RIOT_API.Instance.Init();
        RIOT_API.Instance.SetCurrPlatform(platformLabel.text);

        ErrorMessageInit();

        StartCoroutine(LoadCurrMatch(summonerNameInput.text));
    }
    ///<summary>   To Load userData through Network According to Sequence   </summary>
    public IEnumerator LoadCurrMatch(string SummonerName)
    {
        Loading(true);
        yield return RIOT_API.Instance.GetSummonerByName(SummonerName);
        Loading(false);

        if (RIOT_API.Instance.Summoner.IsNull)
        {
            ChangeTextOnPanel(errorPanel, "Can you check your name again?");  // to change error message
            errorPanel.SetActive(true);  // display error panel
            yield break;
        }

        Loading(true);
        yield return RIOT_API.Instance.GetSpectatorBySummoner(RIOT_API.Instance.Summoner.id);
        Loading(false);

        if (RIOT_API.Instance.Spectator.IsNull)
        {
            ChangeTextOnPanel(errorPanel, "You're not playing, are you?");  // to change error message
            errorPanel.SetActive(true);  // display error panel
            yield break;
        }
        else if (RIOT_API.Instance.Spectator.gameMode != "CLASSIC")
        {
            ChangeTextOnPanel(errorPanel, "Sorry, ONLY for classic game.");  // to change error message
            errorPanel.SetActive(true);  // display error panel
            yield break;
        }
        
        GotoOtherScene("OnGame Scene");
    }
}
