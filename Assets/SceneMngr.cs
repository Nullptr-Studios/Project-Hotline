using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    /// <summary>
    /// Delegate for handling civilian panic events.
    /// </summary>
    public delegate void CivilianPanic();

    /// <summary>
    /// Static delegate instance for civilian panic events.
    /// </summary>
    public static CivilianPanic CivilianPanicDelegate;

    /// <summary>
    /// List of exit nodes in the scene.
    /// </summary>
    public static List<GameObject> ExitNodes;

    /// <summary>
    /// Data for the scene.
    /// </summary>
    public SceneData SceneData;

    #if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
    #endif

    /// <summary>
    /// Initializes the scene manager and loads initial scenes.
    /// </summary>
    void Start()
    {
        ExitNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("ExitNode"));

        foreach(var scene in SceneData.sceneObjects)
        {
            if(scene.isInitialyLoaded)
            {
                LoadScenePrivateAsync(scene.sceneObject);
            }
        }
    }

    /// <summary>
    /// Unloads all scenes when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            UnloadScenePrivateAsync(scene.sceneObject);
        }
    }

    /// <summary>
    /// Unloads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to unload.</param>
    private void UnloadScenePrivateAsync(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.UnloadSceneAsync(sceneName);
        asyncOper.completed += AsyncOperUnloading_completed;
    }

    /// <summary>
    /// Callback for when a scene has finished unloading.
    /// </summary>
    /// <param name="obj">The async operation.</param>
    private void AsyncOperUnloading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Scene " + obj.ToString() + " finished unloading");
        }
#endif
    }

    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    private void LoadScenePrivateAsync(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOper.completed += AsyncOperLoading_completed;
    }

    /// <summary>
    /// Callback for when a scene has finished loading.
    /// </summary>
    /// <param name="obj">The async operation.</param>
    private void AsyncOperLoading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Scene " + obj.ToString() + " finished loading");
        }
#endif
    }

    /// <summary>
    /// Loads a scene by name.
    /// </summary>
    /// <param name="levelName">The name of the scene to load.</param>
    public void LoadScene(string levelName)
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            if (scene.sceneObject == levelName)
            {
                LoadScenePrivateAsync(scene.sceneObject);
                return;
            }
        }
    }

    /// <summary>
    /// Unloads a scene by name.
    /// </summary>
    /// <param name="levelName">The name of the scene to unload.</param>
    public void UnloadScene(string levelName)
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            if (scene.sceneObject == levelName)
            {
                LoadScenePrivateAsync(scene.sceneObject);
                return;
            }
        }
    }
}