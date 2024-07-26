using UnityEngine;

public class Buffering : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 350;
    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.Rotate(new Vector3(0,0, Time.deltaTime * rotSpeed));
    }
}
