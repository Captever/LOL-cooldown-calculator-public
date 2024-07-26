using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivePanel : UIManager
{
    private RIOT_API.CHAMPIONS_DATA.Data.AdvancedData.Passive passiveInfo; // current passive information

    private long cooldown;

    // Start is called before the first frame update
    void Start()
    {
        ApplyBackground(passiveInfo.image.texture, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // ==== Made for parent Use ====
    // To apply current passive information to variable from Member Panel(parent)
    public void GiveInformationToPassive(RIOT_API.CHAMPIONS_DATA.Data.AdvancedData.Passive currPassive)
    {
        passiveInfo = currPassive;
    }
}
