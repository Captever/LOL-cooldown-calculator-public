using UnityEngine;
using System.Collections;
using TMPro;

public class OptimizingUI : UIManager
{
    [Header("Size")]
    [Tooltip("0:Fullscreen\n" +
             "1:Related to Screen Width\n" +
             "2:Related to Screen Height\n" +
             "3:Related to Parent UI Width/Height\n" +
             "4:Related to Parent UI Width\n" +
             "5:Related to Parent UI Height")]
    public int resizeType;
    [Tooltip("Width Scale is a variable to modify Width")]
    public float widthScale;
    [Tooltip("Height Scale is a variable to modify Height")]
    public float heightScale;

    [Header("Fontsize(Optional)")]
    [Tooltip("These 'TextMeshProUGUI's to be adjusted in relation to the size of This UI")]
    public TextMeshProUGUI[] relatedTextMeshes;
    [Tooltip("0:Related to This UI's Width\n" +
             "1:Related to This UI's Height")]
    public int fontResizeType;
    [Tooltip("Fontsize Scale is a varialbe to modify Fontsize of 'relatedTextMeshes'")]
    public float fontsizeScale;

    [Header("Position")]
    [Tooltip("0:(X, Y) is proportional to FullScreen\n" +
             "1:X and Y are proportional to Screen Width\n" +
             "2:X and Y are proportional to Screen Height\n" +
             "3:X/Y are proportional to Parent UI Width/Height\n" +
             "4:X and Y are proportional to Parent UI Width\n" +
             "5:X and Y are proportional to Parent UI Height\n" +
             "6:X/Y are proportional to Current UI Width/Height\n" +
             "7:X and Y are proportional to Current UI Width\n" +
             "8:X and Y are proportional to Current UI Height")]
    public int offsetType;
    [Tooltip("Offset From Anchor Point")]
    public Vector2 offset;


    private RectTransform parentRT = null;

    // Start is called before the first frame update
    void Start()
    {
        ApplyCurrRT();

        StartCoroutine(Optimizing());
    }
    bool IsAffectedByParent()
    {
        return (resizeType >= 3 && resizeType < 6) || (offsetType >= 3 && offsetType < 9);
    }
    IEnumerator Optimizing()
    {
        // have to do only after finishing parent work
        OptimizingUI parentC = transform.parent.GetComponent<OptimizingUI>();
        if (parentC != null && IsAffectedByParent()) yield return new WaitUntil(() => parentC.enabled == false);

        // on Optimizing
        Resize();
        Reposition();

        // deactivate after optimizing is complete
        GetComponent<OptimizingUI>().enabled = false;

        yield return true;
    }

    // About Size
    void Resize()
    {
        rt.sizeDelta = SizeToResize();
        for (int i = 0; i < relatedTextMeshes.Length; i++)
        {
            relatedTextMeshes[i].fontSize = FontsizeToResize();
        }
    }
    Vector2 SizeToResize()
    {
        Vector2 ret;

        switch (resizeType)
        {
            case 0: // Fullscreen
                ret = new Vector2(Screen.width, Screen.height);
                break;
            case 1: // Related to Screen Width
                ret = new Vector2(Screen.width, Screen.width);
                break;
            case 2: // Related to Screen Height
                ret = new Vector2(Screen.height, Screen.height);
                break;
            case 3: // Related to Parent UI Width/Height
                ApplyParentRT();
                ret = parentRT.sizeDelta;
                break;
            case 4: // Related to Parent UI Width
                ApplyParentRT();
                ret = new Vector2(parentRT.sizeDelta.x, parentRT.sizeDelta.x);
                break;
            case 5: // Related to Parent UI Height
                ApplyParentRT();
                ret = new Vector2(parentRT.sizeDelta.y, parentRT.sizeDelta.y);
                break;
            default:
                ret = new Vector2();
                break;
        }

        ret *= new Vector2(widthScale, heightScale);

        return ret;
    }
    float FontsizeToResize()
    {
        float ret;

        switch (fontResizeType)
        {
            case 0: // Related to This UI's Width
                ret = rt.sizeDelta.x;
                break;
            case 1: // Related to This UI's Height
                ret = rt.sizeDelta.y;
                break;
            default:
                ret = 0;
                break;
        }

        ret *= fontsizeScale;

        return ret;
    }

    // About Position
    void Reposition()
    {
        rt.anchoredPosition = PositionToMove();
    }
    Vector2 PositionToMove()
    {
        Vector2 ret;

        switch (offsetType)
        {
            case 0: // proportional to FullScreen
                ret = new Vector2(Screen.width, Screen.height);
                break;
            case 1: // X and Y are proportional to Screen Width
                ret = new Vector2(Screen.width, Screen.width);
                break;
            case 2: // X and Y are proportional to Screen Height
                ret = new Vector2(Screen.height, Screen.height);
                break;
            case 3: // X/Y are proportional to Parent UI Width/Height
                ApplyParentRT();
                ret = parentRT.sizeDelta;
                break;
            case 4: // X and Y are proportional to Parent UI Width
                ApplyParentRT();
                ret = new Vector2(parentRT.sizeDelta.x, parentRT.sizeDelta.x);
                break;
            case 5: // X and Y are proportional to Parent UI Height
                ApplyParentRT();
                ret = new Vector2(parentRT.sizeDelta.y, parentRT.sizeDelta.y);
                break;
            case 6: // X/Y are proportional to Current UI Width/Height
                ret = rt.sizeDelta;
                break;
            case 7: // X and Y are proportional to Current UI Width
                ret = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x);
                break;
            case 8: // X and Y are proportional to Current UI Height
                ret = new Vector2(rt.sizeDelta.y, rt.sizeDelta.y);
                break;
            default:
                ret = new Vector2();
                break;
        }

        ret *= offset;

        return ret;
    }

    private void ApplyParentRT()
    {
        if (parentRT != null) return;

        parentRT = transform.parent.GetComponent<RectTransform>();
    }
}
