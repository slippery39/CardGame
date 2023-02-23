using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncAwaitTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestGetString();   
    }

    private async Task<string> TestGetString()
    {
        string str = await Task.Run<string>(() =>
        {
            Thread.Sleep(10000);
            return "The async string has been retrieved!";
        });

        Debug.Log("Finished " + str);
        return str;
    }
}
