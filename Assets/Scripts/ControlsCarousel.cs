using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // or TMPro if you're using TextMeshPro

public class ControlsCarousel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI uiText; // Or use TMPro.TMP_Text if using TextMeshPro
    public float displayTime = 5f;
    public List<string> messages;

    private void Start()
    {
        if (messages != null && messages.Count > 0)
        {
            StartCoroutine(CarouselText());
        }
    }

    IEnumerator CarouselText()
    {
        int index = 0;
        while (true)
        {
            uiText.text = messages[index];
            index = (index + 1) % messages.Count;
            yield return new WaitForSeconds(displayTime);
        }
    }
}
