using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEventLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public void SetLog(IEnumerable<string> messages)
    {
        text.text = string.Join("\n", messages);
    }
}
