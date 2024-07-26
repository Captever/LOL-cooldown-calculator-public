using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private GameObject cooldownPrefab = null;

    private GameObject gameObject_Cooldown = null;

    public void SetCooldownAndShow(Transform parent, double cooldown)
    {
        if (gameObject_Cooldown != null)
        {
            StopAllCoroutines();
            Destroy(gameObject_Cooldown);
        }

        gameObject_Cooldown = Instantiate(cooldownPrefab, parent);

        StartCoroutine(ShowCooldown((float)cooldown));
    }
    private IEnumerator ShowCooldown(float cooldown)
    {
        float currCooldown = cooldown;

        Image imageComponent_Cooldown = gameObject_Cooldown.GetComponent<Image>();
        TextMeshProUGUI text = gameObject_Cooldown.GetComponentInChildren<TextMeshProUGUI>();
        text.fontSize = GetComponent<RectTransform>().sizeDelta.x * 0.5f;

        while (true)
        {
            currCooldown -= Time.deltaTime;
            yield return imageComponent_Cooldown.fillAmount = currCooldown / cooldown;
            text.text = Mathf.CeilToInt(currCooldown).ToString();

            if(currCooldown <= 0)
            {
                Destroy(gameObject_Cooldown);
                gameObject_Cooldown = null;
                break;
            }
        }
    }
}
