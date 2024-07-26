using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPanel : UIManager
{
    private int myTeam;

    private List<RIOT_API.SPECTATOR.Member> teamInfo = null; // curr team's members information


    // ==== for child ====
    [SerializeField] private GameObject memberPanelPrefab = null;
    private int MemberCount => teamInfo.Count;


    // for reference in sizing and locating memberPanels
    private Vector2 totalSize;
    private float Get_A_MemberPanelHeight => totalSize.y / MemberCount; // to divide height by each member

    private List<GameObject> memberPanels = null;
    private float maxY, minY; // to limit drag


    // Start is called before the first frame update
    void Start()
    {
        TeamPanelInit();
    }


    // To initialize object
    private void TeamPanelInit()
    {
        ApplyCurrRT();

        InitUIs();
    }


    // to initialize all of UI
    private void InitUIs()
    {
        // to apply total size
        ApplyTotalSize();

        teamInfo = RIOT_API.Instance.Spectator.GetTeamMembersOfParticipants(myTeam);

        // initialize memberPanels List
        memberPanels = new List<GameObject>(MemberCount);

        for (int i = 0; i < MemberCount; i++)
        {
            // initialize member object
            GameObject member = Instantiate(memberPanelPrefab, transform);
            // => and name
            member.name = "Member" + i;
            // apply member data to child(member) panel script
            member.GetComponent<MemberPanel>().GiveInformationToMember(teamInfo[i]);


            // ==== About current member's rect ====
            // make rect for current member
            Rect currMemberRect = new Rect(0, Get_A_MemberPanelHeight * (-i + 2),
                                            totalSize.x, Get_A_MemberPanelHeight);
            // put it according to currMemberRect
            AdjustRtPositionAndSizeFromRect(member, currMemberRect);


            // add to list
            memberPanels.Add(member);
        }

        ApplyLimitY();
    }
    

    // ==== About Position ====
    // Apply max/min value to maxY/minY from RectTransform of Objects
    private void ApplyLimitY()
    {
        maxY = GetRtPosition(memberPanels[0]).y;
        minY = GetRtPosition(memberPanels[MemberCount - 1]).y;
    }
    // Apply total size(by rect) to variable(totalSize)
    private void ApplyTotalSize()
    {
        totalSize = rt.sizeDelta;
    }


    // ==== Made for parent Use ====
    // to index current team
    public void GiveIndexToTeam(int currTeamIndex)
    {
        myTeam = currTeamIndex;
    }
    // for the execution procedure
    public bool IsReady()
    {
        if (memberPanels == null || memberPanels.Count != MemberCount) return false;

        bool ret = true;

        foreach(GameObject a in memberPanels)
        {
            ret &= a.GetComponent<MemberPanel>().IsReady();
        }

        return ret;
    }


    // ==== Made for memberPanel Use ====
    // When moving memberPanels, it is not allowed to unlimit drag
    public void AvoidLimitlessDrag(RectTransform _base)
    {
        if (_base.anchoredPosition.y > maxY)
        {
            _base.anchoredPosition = new Vector2(0, maxY);
        }
        else if (_base.anchoredPosition.y < minY)
        {
            _base.anchoredPosition = new Vector2(0, minY);
        }
    }

    // Swap current member to target member(EndDrag - MemberPanel)
    public void SwapMemberData(Vector2 currMemberDefaultPos)
    {
        Vector2 mousePos = GetCenterAnchoredPosFromLocalPos(Input.mousePosition);

        // When points outside the limits are pointed, nothing proceeds
        //   The reason why +- Get_A_MemberPanelHeight * 0.5f is because maxY and minY values' anchor is on center
        if (mousePos.y > maxY + Get_A_MemberPanelHeight * 0.5f ||
            mousePos.y < minY - Get_A_MemberPanelHeight * 0.5f) return;

        // targetMember Designation
        int targetMember = GetNearestMemberByPos(mousePos);
        int currMember = GetNearestMemberByPos(currMemberDefaultPos);

        // Avoid swapping the same members
        if (currMember == targetMember) return;

        // Swap Them
        SwapMemberInTeam(currMember, targetMember);
    }
    // Get the index of member panel closest to the pos(param)
    private int GetNearestMemberByPos(Vector2 pos)
    {
        float nearDist = 20000; // Temporary value
        int nearestIndex = 0;

        for(int i = 0; i < memberPanels.Count; i++)
        {
            float currDist = Vector2.Distance(pos, GetRtPosition(memberPanels[i]));

            if(currDist < nearDist)
            {
                nearDist = currDist;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
    // swap member A for member B by index
    private void SwapMemberInTeam(int aIndex, int bIndex)
    {
        GameObject memberA = memberPanels[aIndex];
        GameObject memberB = memberPanels[bIndex];

        Vector2 tmp = GetRtPosition(memberA);
        memberA.GetComponent<RectTransform>().anchoredPosition = GetRtPosition(memberB);
        memberB.GetComponent<RectTransform>().anchoredPosition = tmp;

        //Debug.Log("Swap [" + aIndex + "] <=> [" + bIndex + "]");
    }
}
