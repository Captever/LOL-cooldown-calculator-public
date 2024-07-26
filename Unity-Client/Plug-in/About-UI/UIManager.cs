using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    protected RectTransform rt;


    protected WaitUntil WaitUntilOptimizingEnd()
    {
        return new WaitUntil(() => GetComponent<OptimizingUI>().enabled == false);
    }


    protected void ApplyCurrRT()
    {
        rt = GetComponent<RectTransform>();
    }


    // ==== About Position ====
    protected Vector2 GetRtPosition(GameObject target)
                => target.GetComponent<RectTransform>().anchoredPosition;

    protected Vector2 GetCenterAnchoredPosFromLocalPos(Vector2 localPos)
                => localPos - new Vector2(Screen.width, Screen.height) * 0.5f;


    // ==== About Adjusting RT ====
    protected void AdjustRtPositionAndSizeFromRect(GameObject objectForRT, Rect param)
    {
        RectTransform currRT = objectForRT.GetComponent<RectTransform>();

        currRT.anchoredPosition = param.position;
        currRT.sizeDelta = param.size;
    }
    protected void AdjustRtPositionAndSizeFromRect(RectTransform rt, Rect param)
    {
        rt.anchoredPosition = param.position;
        rt.sizeDelta = param.size;
    }
    protected void AdjustRtPositionAndSizeFromRect(RectTransform rt, Vector2 posParam, Vector2 sizeParam)
    {
        rt.anchoredPosition = posParam;
        rt.sizeDelta = sizeParam;
    }


    // ==== About Background ====
    // Apply panel background to texture(RawImage)
    protected void ApplyBackground(Texture backgroundForChange, GameObject target)
    {
        target.GetComponent<RawImage>().texture = backgroundForChange;
    }
    // Apply panel background to sprite(Image)
    protected void ApplyBackground(Sprite backgroundForChange, GameObject target)
    {
        target.GetComponent<Image>().sprite = backgroundForChange;
    }
}
