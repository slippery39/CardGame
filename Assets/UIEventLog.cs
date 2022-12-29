using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEventLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ScrollRect scrollRect;
    public void SetLog(IEnumerable<string> messages)
    {
        Debug.Log(string.Join("\n", messages));
        text.SetText(string.Join("\n", messages));
        scrollRect.verticalNormalizedPosition = 0;
        scrollRect.verticalScrollbar.value = 0;
    }

    public void AppendLog(string message)
    {
        text.SetText(text.text+ message + "\n");
    }
}
