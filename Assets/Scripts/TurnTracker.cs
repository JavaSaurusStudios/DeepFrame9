using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTracker : MonoBehaviour
{
    public static TurnTracker INSTANCE;

    public float turnInterval = 3f;
    [SerializeField]
    private float turnTimer;

    public TMPro.TextMeshProUGUI loadingText;
    public Image loadingImage;

    public bool isTurn = false;
    public delegate void OnChangeState(bool turn);
    public OnChangeState onChangeState;

    public void TakeTurn()
    {
        isTurn = false;
        turnTimer = turnInterval;
        loadingImage.fillAmount = 0;
        loadingText.text = "Wait...";
    }

    void Awake()
    {
        INSTANCE = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurn)
        {
            loadingImage.fillAmount = 1;
            loadingText.text = "Ready !";
            turnTimer = turnInterval;
        }
        else if (turnTimer > 0)
        {
            turnTimer -= Time.deltaTime;
            loadingImage.fillAmount = 1 - (turnTimer / turnInterval);
            loadingText.text = "Wait...";
            if (turnTimer <= 0)
            {
                isTurn = true;
            }
        }

    }
}
