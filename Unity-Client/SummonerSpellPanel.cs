using System;
using UnityEngine;

public class SummonerSpellPanel : UIManager
{
    private RIOT_API.SUMMONER_SPELLS_DATA.Data summonerSpellInfo; // current summoner spell information

    private Cooldown script_Cooldown = null;
    private MemberPanel script_MemberPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        SummonerSpellPanelInit();
    }

    // to initialize
    private void SummonerSpellPanelInit()
    {
        ApplyCooldown();
        ApplyMemberPanel();

        ApplyBackground(summonerSpellInfo.image.texture, gameObject);
    }

    // show cooldown when transform is clicked
    public void ShowCooldown()
    {
        double cooldown = summonerSpellInfo.
                            cooldown[Math.Min(
                                summonerSpellInfo.cooldown.Count - 1,
                                script_MemberPanel.GetMemberCurrLevel() - 1)];

        script_Cooldown.SetCooldownAndShow(transform, cooldown);
    }

    // ==== Made for parent Use ====
    // To apply current summoner spell information to variable from Member Panel(parent)
    public void GiveInformationToSummonerSpell(RIOT_API.SUMMONER_SPELLS_DATA.Data currSummonerSpell)
    {
        summonerSpellInfo = currSummonerSpell;
    }


    // To apply script_Cooldown to variable
    private void ApplyCooldown()
    {
        script_Cooldown = GetComponent<Cooldown>();
    }
    // To apply script_Member(from parent) to variable
    private void ApplyMemberPanel()
    {
        script_MemberPanel = GetComponentInParent<MemberPanel>();
    }
}
