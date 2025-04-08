using UnityEngine;

public class DepthTracker : MonoBehaviour
{
    public static DepthTracker INSTANCE;

    public TMPro.TextMeshProUGUI depthText; // Assign in Inspector

    private int _depth = 0;

    public int depth
    {
        set
        {
            _depth = value > _depth ? value : _depth;
            depthText.text = _depth.ToString("D4");
        }
        get
        {
            return _depth;
        }
    }

    void LateUpdate()
    {
        depth = Mathf.CeilToInt(ControllerFeedback.INSTANCE.transform.position.y * -10);
    }

    void Awake()
    {
        INSTANCE = this;
    }

}
