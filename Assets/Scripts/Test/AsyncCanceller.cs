using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Unity has an issue, when you are using async tasks the async tasks will continue even upon exiting play mode in the editor.
 * This script will stop all tasks executing by reloading the assembly.
 * Its a bit of a hack, but until unity offers decent support, this is the most foolproof way of stopping tasks.
 * This thread shows more details : https://forum.unity.com/threads/non-stopping-async-method-after-in-editor-game-is-stopped.558283/
 */
public class AsyncCanceller : MonoBehaviour
{
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeFirstSceneLoad_Static()
    {
        UnityEditor.EditorApplication.playModeStateChanged += state =>
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("[AppInit] Reloading scripts to prevent dangling Tasks from continuing executing in edit mode");
                UnityEditor.EditorUtility.RequestScriptReload();
                // Not sure if needed
                UnityEditor.EditorApplication.isPaused = true;
            }
        };
    }
}
