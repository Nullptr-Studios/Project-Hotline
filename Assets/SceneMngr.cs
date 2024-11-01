using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{

    public SceneData SceneData;
    
    #if UNITY_EDITOR
    [Header("Debug")] 
    [SerializeField] private bool log = false;
    #endif

    // Start is called before the first frame update
    void Start()
    {
        foreach(var scene in SceneData.sceneObjects)
        {
            if(scene.isInitialyLoaded)
            {
                LoadScenePrivateAsync(scene.sceneObject);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var scene in SceneData.sceneObjects)
        {
            UnloadScenePrivateAsync(scene.sceneObject);
        }
    }

    private void UnloadScenePrivateAsync(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.UnloadSceneAsync(sceneName);
        asyncOper.completed += AsyncOperUnloading_completed;
    }

    private void AsyncOperUnloading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Scene " + obj.ToString() + " finished unloading");
        }
#endif
    }

    private void LoadScenePrivateAsync(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOper.completed += AsyncOperLoading_completed;
    }

    private void AsyncOperLoading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Scene " + obj.ToString() + " finished loading");
        }
#endif
    }

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
