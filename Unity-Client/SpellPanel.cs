using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpellPanel : UIManager
{
    [SerializeField] private TextMeshProUGUI spellRankText = null;

    private RIOT_API.CHAMPIONS_DATA.Data.AdvancedData.Spell spellInfo; // current spell information
    private Cooldown script_Cooldown = null;

    private int maxrank;
    private List<string> rankNames;
    private List<int> costs;
    private List<double> cooldowns;

    private int currank;


    // Start is called before the first frame update
    void Start()
    {
        SpellPanelInit();
    }

    // to initialize
    private void SpellPanelInit()
    {
        ApplyCooldown(); // to use script component

        InitDataForUse();

        ChangeCurrRank(0);

        ApplyBackground(spellInfo.image.texture, gameObject);
    }
    private void InitDataForUse()
    {
        maxrank = spellInfo.maxrank;
        rankNames = new List<string>();
        costs = new List<int>();
        cooldowns = new List<double>();

        for(int i = 0; i <= maxrank; i++)
        {
            if (i != 0)
            {
                rankNames.Add(i.ToString());
                costs.Add(spellInfo.cost[i - 1]);
                cooldowns.Add(spellInfo.cooldown[i - 1]);
            }
            else  // if(i == 0)
            {
                rankNames.Add("N");
                costs.Add(0);
                cooldowns.Add(0);
            }
        }
    }

    // show cooldown when transform is clicked
    public void ShowCooldown()
    {
        script_Cooldown.SetCooldownAndShow(transform, cooldowns[currank]);
    }

    // for button - to step to next rank(true is to up, false is to down)
    public void StepNextRank(bool toUp)
    {
        int nextRank = currank;

        if (toUp)
        {
            nextRank++;
            if (nextRank > maxrank) return;
        }
        else
        {
            nextRank--;
            if (nextRank < 0) return;
        }

        ChangeCurrRank(nextRank);
    }


    // change curr rank and apply associated with that
    private void ChangeCurrRank(int targetRank)
    {
        currank = targetRank;
        ApplyRankText(targetRank);
    }
    // to apply rank text to text component
    private void ApplyRankText(int targetRank)
    {
        spellRankText.text = rankNames[targetRank];
    }

    // ==== Made for parent Use ====
    // To apply current spell information to variable from Member Panel(parent)
    public void GiveInformationToSpell(RIOT_API.CHAMPIONS_DATA.Data.AdvancedData.Spell currSpell)
    {
        spellInfo = currSpell;
    }


    // To apply script_cooldown to variable
    private void ApplyCooldown()
    {
        script_Cooldown = GetComponent<Cooldown>();
    }
}
