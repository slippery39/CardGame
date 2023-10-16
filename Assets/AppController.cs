using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all the different screens and menus for the game.
/// </summary>
public class AppController : MonoBehaviour
{
    [SerializeField]
    private Scene _mainMenu;
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneWithCallback("MainMenu", (gameObjects) =>
        {
            var mainMenuManager = gameObjects
            .Select(go => go.GetComponentInChildren<GameSetupController>())
            .First(go => go != null);

            if (mainMenuManager == null)
            {
                Debug.LogError("Could not find the GameSetupController in the MainMenu scene");
            }
            else
            {
                mainMenuManager.Initialize(this);
            }
        }
        ));
    }
    public void StartGame(GameSetupOptions options)
    {
        StartCoroutine(LoadSceneWithCallback("3DGameView", (gameObjects) =>
        {
            var uiController = gameObjects
            .Select(go => go.GetComponentInChildren<UI3DController>())
            .First(go => go != null);

            if (uiController == null)
            {
                Debug.LogError("Could not find the Ui3DController in the 3D Game View Scene");
            }
            else
            {
                uiController.Initialize(this);
                uiController.StartGame(options);
            }
        }
        ));
    }

    private IEnumerator LoadSceneWithCallback(string sceneName, Action<GameObject[]> onComplete)
    {
        Debug.Log($"Trying to load scene ${sceneName}...");
        var loadTask = SceneManager.LoadSceneAsync(sceneName);
        while (!loadTask.isDone)
        {
            Debug.Log("Still loading...");
            yield return null;
        }

        Debug.Log($"Done loading ${sceneName}");

        var gameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
        onComplete(gameObjects);
    }
}
