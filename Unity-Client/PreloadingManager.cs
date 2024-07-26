using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadingManager : EachSceneManager
{
    [SerializeField] private GameObject openingPanel = null;
    [SerializeField] private GameObject logo_CI = null;

    public bool IsPreloadingOver { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        PreloadingInit();

        StartCoroutine(ExecutingProcedure());
    }


    // initialize scene
    private void PreloadingInit()
    {
        if (GameManager.Instance.GAME_STATE == GameManager.GameState.Preloading) return;  // To prevent duplicate

        GameManager.Instance.GAME_STATE = GameManager.GameState.Preloading;

        logo_CI.SetActive(false);
        openingPanel.SetActive(false);

        IsPreloadingOver = false;
    }


    // executing all procedure(like prepreperation, ...)
    private IEnumerator ExecutingProcedure()
    {
        StartCoroutine(Prepreperation());  // To speed up loading, start pre-preperation before the opening is displayed

        StartCoroutine(Opening());

        yield return new WaitUntil(() => (IsPreloadingOver == true) && (IsOnLoading == true));
        GotoOtherScene("Info Scene");  // go to Info Scene after preloading is over
    }
    // for pre-preperation
    private IEnumerator Prepreperation()
    {
        yield return StartCoroutine(RIOT_API.Instance.GetVersions()); // to get api versions
        yield return StartCoroutine(RIOT_API.Instance.GetChampionsData()); // to get champions data
        yield return StartCoroutine(RIOT_API.Instance.GetSummonerSpellsData()); // to get summoner spells data

        IsPreloadingOver = true;
        yield return 0;
    }
    // display opening credits like production company CI, ... and loading panel
    private IEnumerator Opening()
    {
        openingPanel.SetActive(true);

        // display CI during 2 seconds
        logo_CI.SetActive(true);
        logo_CI.AddComponent<FadingDuring2sec>();

        yield return new WaitForSeconds(2.5f); // 2(s) = fading time + 0.5(s) = delay(spare) time

        // end of opening
        Destroy(openingPanel);
        Loading(true); // show loading panel until scene changing
        yield return 0;
    }
}
