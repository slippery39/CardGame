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
        text.text = string.Join("\n", messages);
        scrollRect.verticalNormalizedPosition = 0;
        scrollRect.verticalScrollbar.value = 0;
    }
}
