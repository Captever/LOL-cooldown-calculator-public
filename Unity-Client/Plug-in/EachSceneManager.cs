using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EachSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject onLoadingCanvas = null;

    protected bool IsOnLoading => onLoadingCanvas.activeSelf;

    // using only for override on child class
    ///<summary>   To display a loading panel(for each Scene) to the user.   </summary>
    protected void Loading(bool onLoading)
    {
        onLoadingCanvas.SetActive(onLoading);
    }

    ///<summary>   To make the network work only when it does nothing.   </summary>
    protected WaitUntil WaitUntilNetworkFree()
    {
        return new WaitUntil(() => GameManager.Instance.NETWORK_PROCESS == GameManager.NetworkProcess.None);
    }

    ///<summary>   To change text on panel's child text gameObject(have to use TextMeshPro).   </summary>
    /// <param name="panel">  Parent panel  </param>
    /// <param name="text">  Text content to change  </param>
    protected void ChangeTextOnPanel(Transform panel, string text)
    {
        panel.Find("Text").GetComponent<TextMeshProUGUI>().SetText(text);
    }
    ///<summary>   To change text on panel's child text gameObject(have to use TextMeshPro).   </summary>
    /// <param name="panel">  Parent panel  </param>
    /// <param name="text">  Text content to change  </param>
    protected void ChangeTextOnPanel(GameObject panel, string text)
    {
        panel.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(text);
    }

    ///<summary>   To convert the scene by name   </summary>
    /// <param name="sceneName">  scene name to change  </param>
    protected void GotoOtherScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
