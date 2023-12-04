using Assets.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all the different screens and menus for the game.
/// </summary>
public class AppController : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingScreen;

    [SerializeField]
    private EventSystem _defaultEventSystem;
    [SerializeField]
    private Camera _defaultMainCamera;

    public static AppController Instance { get; private set; }

    public void Awake()
    {
        //Only allow one AppController to exist
        if (Instance == null)
        {
            Instance= this;
        }
        else
        {
            DestroyImmediate(this);
        }

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

    private void EnableEssentialObjectsInScene(GameObject[] rootObjectsInScene)
    {
        bool eventSystemFound = false;
        bool cameraFound = false;
        foreach (var go in rootObjectsInScene)
        {
            if (go.GetComponentInChildren<EventSystem>(true) != null)
            {
                eventSystemFound = true;
                go.SetActive(true);
                _defaultEventSystem.gameObject.SetActive(false);
            }

            var cameras = go.GetComponentsInChildren<Camera>();
            if (cameras.Length > 0)
            {
                foreach (var cam in cameras)
                {
                    if (cam.tag == "MainCamera")
                    {
                        cameraFound = true;
                        cam.gameObject.SetActive(true);
                        _defaultMainCamera.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (!eventSystemFound)
        {
            _defaultEventSystem.gameObject.SetActive(true);
        }
        if (!cameraFound)
        {
            _defaultMainCamera.gameObject.SetActive(true);
        }
    }

    private IEnumerator LoadSceneWithCallback(string sceneName, Action<GameObject[]> onComplete)
    {
        Debug.Log($"Trying to load scene ${sceneName}...");
        _loadingScreen.SetActive(true);
        var loadTask = SceneManager.LoadSceneAsync(sceneName);
        while (!loadTask.isDone)
        {
            Debug.Log("Still loading...");
            yield return null;
        }
        var gameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
        EnableEssentialObjectsInScene(gameObjects);
        _loadingScreen.SetActive(false);

        Debug.Log($"Done loading ${sceneName}");
        onComplete(gameObjects);
    }
}
