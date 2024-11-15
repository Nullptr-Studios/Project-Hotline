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

    public static SCameraVariables ActiveSceneCameraVars;

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
    void Awake()
    {
        if(SceneData == null)
        {
            Debug.LogError("SceneData is not assigned in " + gameObject.name);
            //Destroy(this);
            //return;
        }

        ExitNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("ExitNode"));

        foreach(var scene in SceneData.sceneObjects)
        {
            if(scene.isInitialyLoaded)
            {
                LoadScenePrivateAsync(scene.sceneObject);
                
                if(scene.isInitialyActive)
                    ActiveSceneCameraVars = scene.cameraBehaviour;
            }
        }
    }

    /// <summary>
    /// Unloads all scenes when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if(SceneData == null)
            return;

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
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            if (log) Debug.Log($"Scene {sceneName} is not loaded, aborting unload");
            return;
        }
        AsyncOperation asyncOper = SceneManager.UnloadSceneAsync(sceneName);
        if (asyncOper != null) 
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
        obj.completed -= AsyncOperUnloading_completed;
        
        for (int i = 0; i < SceneData.sceneObjects.Count; i++)
        {
            if (SceneData.sceneObjects[i].sceneObject == obj.ToString())
            {
                var sScene = SceneData.sceneObjects[i];
                sScene.isLoaded = false;
                SceneData.sceneObjects[i] = sScene;
                break;
            }
        }
    }

    /// <summary>
    /// Loads a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    private void LoadScenePrivateAsync(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOper!.completed += AsyncOperLoading_completed;
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
        obj.completed -= AsyncOperLoading_completed;

        for (int i = 0; i < SceneData.sceneObjects.Count; i++)
        {
            if (SceneData.sceneObjects[i].sceneObject == obj.ToString())
            {
                var sScene = SceneData.sceneObjects[i];
                sScene.isLoaded = true;
                SceneData.sceneObjects[i] = sScene;
                break;
            }
        }
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
            if (scene.sceneObject == levelName /*&& scene.isLoaded*/)
            {
                UnloadScenePrivateAsync(scene.sceneObject);
                return;
            }
        }
    }
    
    public void SetCameraActiveScene(string sceneName)
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            if (scene.sceneObject == sceneName)
            {
                ActiveSceneCameraVars = scene.cameraBehaviour;
                return;
            }
        }
    }

    /// <summary>
    /// reloads the scenes as they were initially loaded.
    /// </summary>
    public void ReloadScenes()
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            if (scene.isInitialyLoaded)
                continue;
            if(scene.isLoaded)
                UnloadScenePrivateAsync(scene.sceneObject);
        }
    }
}