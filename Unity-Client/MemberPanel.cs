using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemberPanel : UIManager
{
    private TeamPanel script_TeamPanel = null; // parent panel
    private Vector2 defaultPos; // To go back member's original(default) position after the end of drag
    private int defaultSiblingIndex;
    private RIOT_API.SPECTATOR.Member memberInfo; // curr member's information

    private int currLevel;

    // ==== for child ====
    [SerializeField] private GameObject levelDisplayPanelPrefab = null;
    [SerializeField] private GameObject portraitPanelPrefab = null;
    //[SerializeField] private GameObject passivePanelPrefab = null;
    [SerializeField] private GameObject spellPanelPrefab = null;
    [SerializeField] private GameObject summonerSpellPanelPrefab = null;

    // about child name
    private readonly string[] spellNames = new string[] { "Q", "W", "E", "R" };
    private readonly string[] summonerSpellNames = new string[] { "D", "F" };
    // for reference in counting child
    private int SpellCount => spellNames.Length;
    private int SummonerSpellCount => summonerSpellNames.Length;

    private GameObject levelDisplayPanel = null;
    private GameObject portraitPanel = null;
    //private GameObject passivePanel = null;
    private List<GameObject> spellPanels = null;
    private List<GameObject> summonerSpellPanels = null;

    // for reference in sizing and locating child panels
    private Vector2 totalSize;
    private float Get_A_GeneralPanelLength => totalSize.x * 0.095f;
    private float Get_A_GeneralPanelGap => Get_A_GeneralPanelLength * 0.1f;
    private float Get_A_PortraitPanelLength => Get_A_GeneralPanelLength * 1.8f;
    private float Get_A_PortraitPanelGap => Get_A_PortraitPanelLength * 0.09f;
    //private float Get_A_PassivePanelLength => Get_A_GeneralPanelLength;
    private float Get_A_SpellPanelLength => Get_A_GeneralPanelLength * 1.2f;
    private float Get_A_SpellPanelGap => Get_A_SpellPanelLength * 0.1f;
    private float Get_A_SideEdgeGap => Screen.width * 0.02f;


    // Start is called before the first frame update
    void Start()
    {
        MemberPanelInit();
    }


    // To initialize object
    private void MemberPanelInit()
    {
        ApplyCurrRT();
        ApplyTeamPanel();

        ChangeCurrLevel(1);

        isReady = false;
        StartCoroutine(InitUIs());
    }
    public void ChangeCurrLevel(int targetLevel)
    {
        currLevel = targetLevel;
    }

    // ==== About UI ====
    // To initialize portrait, spell, summonerSpell panels
    private IEnumerator InitUIs()
    {
        // to apply total size
        ApplyTotalSize();

        // pre-declaration
        float nextGap; string panelName; Rect panelRect;


        // ==== About current member's champion ====
        string championId = memberInfo.usersChampionInfo.id;
        // get champion splash image
        yield return StartCoroutine(RIOT_API.Instance.GiveChampionSpalsh(championId));
        // apply that splash
        ApplyBackground(memberInfo.usersChampionInfo.defaultSplashImage, gameObject);
        // get advanced champion data(passive, spells, ...)
        yield return StartCoroutine(RIOT_API.Instance.GiveAdvancedChampionData(championId));


        // ==== for portrait panel ==== (do it together here because there is no separate script)
        // get champion portrait image(square)
        yield return StartCoroutine(RIOT_API.Instance.GiveChampionPortraitImage(championId));

        // add GameObject name Portrait(with displaying level)
        panelName = "Portrait";
        nextGap = Get_A_SideEdgeGap + Get_A_PortraitPanelGap;
        panelRect = new Rect(nextGap, 0,
                                Get_A_PortraitPanelLength, Get_A_PortraitPanelLength);
        portraitPanel =
            InstantiatePanelWithNameAndPosition(
                portraitPanelPrefab, transform, panelName, panelRect);

        // adjusting level display panel position
        panelRect.y = -Get_A_PortraitPanelLength * 0.45f;
        panelRect.height = Get_A_PortraitPanelLength * 0.25f;
        levelDisplayPanel =
            InstantiatePanelWithNameAndPosition(
                levelDisplayPanelPrefab, transform, "DisplayLv", panelRect);

        // plus gap
        nextGap += panelRect.size.x + Get_A_PortraitPanelGap;

        // apply that image
        ApplyBackground(memberInfo.usersChampionInfo.image.texture, portraitPanel);


        // ==== for passive panel ====
        /*
        // get champion passive image
        yield return StartCoroutine(RIOT_API.Instance.GiveChampionPassiveImage(championId));

        // add GameObject name Passive
        panelName = "Passive";
        panelRect = new Rect(nextGap, 0,
                                Get_A_GeneralPanelLength, Get_A_GeneralPanelLength);
        passivePanel =
            InstantiatePanelWithNameAndPosition(
                passivePanelPrefab, transform, panelName, panelRect);
        passivePanel.GetComponent<PassivePanel>().
            GiveInformationToPassive(
                memberInfo.usersChampionInfo.advancedData.passive);

        // plus gap
        nextGap += panelRect.size.x + Get_A_GeneralPanelGap;
        */


        // ==== for spell panel ====
        // initialize list
        spellPanels = new List<GameObject>(SpellCount);
        // get member's spells information
        List<RIOT_API.CHAMPIONS_DATA.Data.AdvancedData.Spell> spellsInfo
            = memberInfo.usersChampionInfo.advancedData.spells;

        // get champion spells image
        yield return StartCoroutine(RIOT_API.Instance.GiveChampionSpellsImage(championId));

        for (int i = 0; i < SpellCount; i++)
        {
            panelName = "Spell" + spellNames[i];
            panelRect = new Rect(nextGap, 0,
                                    Get_A_SpellPanelLength, Get_A_SpellPanelLength);

            // add GameObject
            spellPanels.Add(
                InstantiatePanelWithNameAndPosition(
                    spellPanelPrefab, transform, panelName, panelRect));
            // give info to current panel's script
            spellPanels[i].GetComponent<SpellPanel>().
                GiveInformationToSpell(spellsInfo[i]);

            // plus gap
            nextGap += panelRect.size.x + Get_A_SpellPanelGap;
        }
        // plus gap(offset from spell panels)
        nextGap += Get_A_GeneralPanelGap;

        // ==== for summonerSpell panel ====
        // initialize list
        summonerSpellPanels = new List<GameObject>(SummonerSpellCount);
        // get member's summoner spells information
        List<RIOT_API.SUMMONER_SPELLS_DATA.Data> summonerSpellsInfo
            = memberInfo.userInfo.GetSummonerSpellsInfoOfMember();

        for (int i = 0; i < SummonerSpellCount; i++)
        {
            panelName = "SummonerSpell" + summonerSpellNames[i];
            panelRect = new Rect(nextGap, 0,
                                    Get_A_SpellPanelLength, Get_A_SpellPanelLength);

            // add GameObject
            summonerSpellPanels.Add(
                InstantiatePanelWithNameAndPosition(
                    summonerSpellPanelPrefab, transform, panelName, panelRect));
            // give info to current panel's script
            summonerSpellPanels[i].GetComponent<SummonerSpellPanel>().
                GiveInformationToSummonerSpell(summonerSpellsInfo[i]);

            // plus gap
            nextGap += panelRect.size.x + Get_A_SpellPanelGap;
        }

        isReady = true;
    }
    // Apply total size(by rect) to variable(totalSize)
    private void ApplyTotalSize()
    {
        totalSize = rt.sizeDelta;
    }
    // Add new object with designated name and position
    private GameObject InstantiatePanelWithNameAndPosition(GameObject prefab, Transform parent, string panelName, Rect panelRect)
    {
        GameObject ret = Instantiate(prefab, parent);
        ret.name = panelName;

        AdjustRtPositionAndSizeFromRect(ret, panelRect);

        return ret;
    }


    // ==== Made for parent Use ====
    // To apply current member's information to variable from Team Panel(parent)
    public void GiveInformationToMember(RIOT_API.SPECTATOR.Member currMemberInfo)
    {
        memberInfo = currMemberInfo;
    }
    // for the execution procedure
    private bool isReady = false;
    public bool IsReady()
    {
        // TODO : 로딩 기간 명확하게 변경할 것(하위 스크립트에서도 레디를 받도록)
        return isReady;
    }


    // ==== Made for child Use ====
    // to get member level
    public int GetMemberCurrLevel()
    {
        return currLevel;
    }


    // ==== Event Trigger ====
    public void BeginDrag()
    {
        defaultPos = rt.anchoredPosition;

        // Variable to return to default sibling index
        defaultSiblingIndex = transform.GetSiblingIndex();
        // To be shown at the top order
        transform.SetAsLastSibling();
    }
    public void Drag()
    {
        rt.anchoredPosition = new Vector2(0, Input.mousePosition.y - Screen.height * 0.5f); // because of anchor gap(anchor(0, 0) = cetner on canvas)
        script_TeamPanel.AvoidLimitlessDrag(rt);
    }
    public void EndDrag()
    {
        rt.anchoredPosition = defaultPos;
        script_TeamPanel.SwapMemberData(defaultPos);

        // Return to default sibling index
        transform.SetSiblingIndex(defaultSiblingIndex);
    }


    // To apply teamPanel script to variable
    private void ApplyTeamPanel()
    {
        script_TeamPanel = GetComponentInParent<TeamPanel>();
    }
}
