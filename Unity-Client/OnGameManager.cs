using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class OnGameManager : EachSceneManager
{
    [SerializeField] private GameObject wholeTeamObject = null;
    [SerializeField] private GameObject teamPanelPrefab = null;
    [SerializeField] private TextMeshProUGUI teamNameText = null;

    public List<string> teamNames;
    public List<Color> teamColors;
    private int TeamCount => teamNames.Count;
    private int currTeam;

    private List<GameObject> teamPanels = null;

    private Coroutine coroutineToMove = null;

    // Start is called before the first frame update
    void Start()
    {
        OnGameInit();
    }


    // initialize scene
    private void OnGameInit()
    {
        if (GameManager.Instance.GAME_STATE == GameManager.GameState.OnGame) return;  // To prevent duplicate

        GameManager.Instance.GAME_STATE = GameManager.GameState.OnGame;

        StartCoroutine(Preloading());

        ChangeCurrTeam(0); // init
    }
    private IEnumerator Preloading()
    {
        Loading(true);

        InitUIs(TeamCount);

        yield return new WaitUntil(() => IsReady() == true);

        Loading(false);
    }


    // To initialize team panel
    private void InitUIs(int count)
    {
        teamPanels = new List<GameObject>(count);

        for(int i = 0; i < count; i++)
        {
            GameObject team = Instantiate(teamPanelPrefab, wholeTeamObject.transform);
            team.name = "Team" + i;

            // apply team index to child(team) panel script
            team.GetComponent<TeamPanel>().GiveIndexToTeam(i + 1);
            team.GetComponent<OptimizingUI>().offset = new Vector2(i, 0);

            teamPanels.Add(team);
        }
    }

    // for button - to move to next team(true is to right, false is to left)
    public void MoveNextTeam(bool toRight)
    {
        int nextTeam;

        if (toRight)
        {
            nextTeam = currTeam + 1;
            if (nextTeam >= TeamCount) return;
        }
        else
        {
            nextTeam = currTeam - 1;
            if (nextTeam < 0) return;
        }

        if (coroutineToMove != null) StopCoroutine(coroutineToMove);


        coroutineToMove = StartCoroutine(
                                MoveObject(wholeTeamObject,
                                    -GetDistanceXfromZero
                                        (teamPanels[nextTeam]),
                                    0.5f));

        ChangeCurrTeam(nextTeam);
    }
    private float GetDistanceXfromZero(GameObject target)
    {
        return
            wholeTeamObject.GetComponent<RectTransform>().anchoredPosition.x +
            target.GetComponent<RectTransform>().anchoredPosition.x;
    }
    private IEnumerator MoveObject(GameObject target, float offsetX, float duration)
    {
        float moveSpeed = offsetX / duration;
        RectTransform targetRT = target.GetComponent<RectTransform>();
        float goalX = targetRT.anchoredPosition.x + offsetX;

        while (true)
        {
            yield return targetRT.anchoredPosition +=
                            new Vector2(Time.deltaTime * moveSpeed, 0);

            if(offsetX < 0 && targetRT.anchoredPosition.x <= goalX ||
                offsetX > 0 && targetRT.anchoredPosition.x >= goalX)
            {
                targetRT.anchoredPosition =
                    new Vector2(goalX, targetRT.anchoredPosition.y);

                coroutineToMove = null;

                break;
            }
        }
    }

    // for the execution procedure
    private bool IsReady()
    {
        if (teamPanels == null || teamPanels.Count != TeamCount) return false;

        bool ret = true;

        foreach (GameObject a in teamPanels)
        {
            ret &= a.GetComponent<TeamPanel>().IsReady();
        }

        return ret;
    }

    private void ChangeCurrTeam(int targetTeam)
    {
        currTeam = targetTeam;
        ApplyTeamName(targetTeam);
        ApplyTeamColor(targetTeam);
    }
    // to apply team name to text component
    private void ApplyTeamName(int currTeam)
    {
        teamNameText.text = teamNames[currTeam];
    }
    // to apply team color to text component
    private void ApplyTeamColor(int currTeam)
    {
        teamNameText.color = teamColors[currTeam];
    }
}