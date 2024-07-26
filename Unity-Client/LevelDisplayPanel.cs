using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelDisplayPanel : MonoBehaviour
{
    private MemberPanel script_MemberPanel = null;

    [SerializeField] private TextMeshProUGUI levelText = null;

    public int maxLevel = 18;
    public int minLevel = 1;
    private int currLevel;

    // Start is called before the first frame update
    void Start()
    {
        LevelDisplayInit();
    }

    private void LevelDisplayInit()
    {
        ApplyMemberPanel();
        ChangeCurrLevel(script_MemberPanel.GetMemberCurrLevel());
    }
    private void ChangeCurrLevel(int targetLevel)
    {
        currLevel = targetLevel;

        // apply current level to parent object
        script_MemberPanel.ChangeCurrLevel(targetLevel);

        ApplyLevelText(targetLevel);
    }
    private void ApplyLevelText(int targetLevel)
    {
        levelText.text = "Lv." + targetLevel.ToString();
    }


    // for button - to step to next level(true is to up, false is to down)
    public void StepNextLevel(bool toUp)
    {
        int nextLevel = currLevel;

        if (toUp)
        {
            nextLevel++;
            if (nextLevel > maxLevel) return;
        }
        else
        {
            nextLevel--;
            if (nextLevel < minLevel) return;
        }

        ChangeCurrLevel(nextLevel);
    }


    private void ApplyMemberPanel()
    {
        script_MemberPanel = GetComponentInParent<MemberPanel>();
    }
}
