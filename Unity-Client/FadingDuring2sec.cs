using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Only for Raw Image
public class FadingDuring2sec : MonoBehaviour
{
    private RawImage ri;

    private float fadeInDelay = 0.5f, fadeOutDelay = 0.5f;
    private float displayTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        ri = GetComponent<RawImage>();
        ri.color = new Color(1, 1, 1, 0); // to transparent

        StartCoroutine(FadeInOut(displayTime));
    }

    IEnumerator FadeInOut(float displayTime)
    {
        Color cTemp = ri.color;
        while (cTemp.a < 1)
        {
            cTemp.a += Time.deltaTime / fadeInDelay;
            if (cTemp.a >= 1) cTemp.a = 1;

            ri.color = cTemp;

            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        while (cTemp.a > 0)
        {
            cTemp.a -= Time.deltaTime / fadeOutDelay;
            if (cTemp.a <= 0) cTemp.a = 0;

            ri.color = cTemp;

            yield return null;
        }
    }
}
