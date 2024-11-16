using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    public delegate void CivilianPanic();
    public static CivilianPanic CivilianPanicDelegate;
    public static List<GameObject> ExitNodes;
    public static SCameraVariables ActiveSceneCameraVars;
    public SceneData SceneData;

    private static string _checkpointActiveScene;
    private static List<string> _checkpointScenes;
    private static SceneData _sceneData;
    private static Dictionary<string, bool> loadedScene;
    private static Vector2 _restartPos;
    private static int _checkpointIndex = 0;
    private static GameObject _player;
    private static PlayerHealth _playerHealth;

    void Awake()
    {
        if (SceneData == null)
        {
            Debug.LogError("SceneData is not assigned in " + gameObject.name);
            return;
        }

        _sceneData = SceneData;
        loadedScene = new Dictionary<string, bool>();
        ExitNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("ExitNode"));

        foreach (var scene in _sceneData.sceneObjects)
        {
            loadedScene.Add(scene.sceneObject, false);
            loadedScene.Add(scene.EnemyScene, false);
            if (scene.isInitialyLoaded)
            {
                LoadScenePrivateAsync(scene.sceneObject);
                if (scene.isInitialyActive)
                {
                    ActiveSceneCameraVars = scene.cameraBehaviour;
                    if (!string.IsNullOrEmpty(scene.EnemyScene))
                        LoadScenePrivateAsync(scene.EnemyScene);
                }
            }
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _restartPos = _player.transform.position;
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    private void OnDestroy()
    {
        if (_sceneData == null) return;

        foreach (var scene in _sceneData.sceneObjects)
        {
            if (loadedScene[scene.sceneObject])
                UnloadScenePrivateAsync(scene.sceneObject);
        }
    }

    private static void UnloadScenePrivateAsync(string sceneName)
    {
        var asyncOper = SceneManager.UnloadSceneAsync(sceneName);
        if (asyncOper != null)
            asyncOper.completed += AsyncOperUnloading_completed;

        loadedScene[sceneName] = false;
    }

    private static void AsyncOperUnloading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        Debug.Log("Scene " + obj.ToString() + " finished unloading");
#endif
        obj.completed -= AsyncOperUnloading_completed;
    }

    private static void LoadScenePrivateAsync(string sceneName)
    {
        var asyncOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (asyncOper != null)
            asyncOper.completed += AsyncOperLoading_completed;

        loadedScene[sceneName] = true;
    }

    private static void AsyncOperLoading_completed(AsyncOperation obj)
    {
#if UNITY_EDITOR
        Debug.Log("Scene " + obj.ToString() + " finished loading");
#endif
        obj.completed -= AsyncOperLoading_completed;
    }

    public static void LoadScene(string levelName)
    {
        foreach (var scene in _sceneData.sceneObjects)
        {
            if (scene.sceneObject == levelName)
            {
                LoadScenePrivateAsync(scene.sceneObject);
                return;
            }
        }
    }

    public static void UnloadScene(string levelName)
    {
        foreach (var scene in _sceneData.sceneObjects)
        {
            if (scene.sceneObject == levelName && loadedScene[scene.sceneObject])
            {
                UnloadScenePrivateAsync(scene.sceneObject);
                if (!string.IsNullOrEmpty(scene.EnemyScene))
                    UnloadScenePrivateAsync(scene.EnemyScene);
                return;
            }
        }
    }

    public static void SetActiveScene(string sceneName)
    {
        foreach (var scene in _sceneData.sceneObjects)
        {
            if (scene.sceneObject == sceneName)
            {
                ActiveSceneCameraVars = scene.cameraBehaviour;
                if (!string.IsNullOrEmpty(scene.EnemyScene))
                    LoadScenePrivateAsync(scene.EnemyScene);
                return;
            }
        }
    }

    public static void AddCurrentCheckpoint(Vector2 checkpointPos, List<string> loadedScenes, string activeScene)
    {
        _restartPos = checkpointPos;
        _checkpointScenes = loadedScenes;
        _checkpointActiveScene = activeScene;
        _checkpointIndex++;
    }

    public static void Reload()
    {
        ResourceManager.GetCorpsePool().Dispose();
        ResourceManager.GetCivilianCorpsePool().Dispose();
        
        _player.SetActive(true);
        _player.transform.position = _restartPos;
        _playerHealth.RestartGame();

        foreach (var scene in _sceneData.sceneObjects)
        {
            if (!string.IsNullOrEmpty(scene.EnemyScene) && loadedScene[scene.EnemyScene])
                UnloadScenePrivateAsync(scene.EnemyScene);

            if (_checkpointIndex == 0)
            {
                if (scene.isInitialyLoaded && !loadedScene[scene.sceneObject])
                    LoadScenePrivateAsync(scene.sceneObject);

                if (loadedScene[scene.sceneObject] && !scene.isInitialyLoaded)
                    UnloadScenePrivateAsync(scene.sceneObject);

                if (scene.isInitialyActive)
                    SetActiveScene(scene.sceneObject);
            }
            else
            {
                if (_checkpointScenes.Contains(scene.sceneObject))
                {
                    if (!loadedScene[scene.sceneObject])
                        LoadScenePrivateAsync(scene.sceneObject);

                    if (!string.IsNullOrEmpty(scene.EnemyScene) && _checkpointScenes.Contains(_checkpointActiveScene))
                        SetActiveScene(scene.EnemyScene);
                }
                else
                {
                    if (loadedScene[scene.sceneObject])
                        UnloadScenePrivateAsync(scene.sceneObject);
                }
            }
        }
        
    }
}