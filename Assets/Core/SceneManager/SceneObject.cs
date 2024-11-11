using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Represents a scene object in the game.
/// </summary>
[System.Serializable]
public class SceneObject
{
    /// <summary>
    /// The name of the scene.
    /// </summary>
    [SerializeField]
    private string m_SceneName;

    /// <summary>
    /// Implicit conversion from SceneObject to string.
    /// </summary>
    /// <param name="sceneObject">The SceneObject instance.</param>
    /// <returns>The name of the scene.</returns>
    public static implicit operator string(SceneObject sceneObject)
    {
        return sceneObject.m_SceneName;
    }

    /// <summary>
    /// Implicit conversion from string to SceneObject.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    /// <returns>A new SceneObject instance with the specified scene name.</returns>
    public static implicit operator SceneObject(string sceneName)
    {
        return new SceneObject() { m_SceneName = sceneName };
    }
}

#if UNITY_EDITOR
/// <summary>
/// Custom property drawer for SceneObject in the Unity Editor.
/// </summary>
[CustomPropertyDrawer(typeof(SceneObject))]
public class SceneObjectEditor : PropertyDrawer
{
    /// <summary>
    /// Retrieves the SceneAsset associated with the given scene name.
    /// </summary>
    /// <param name="sceneObjectName">The name of the scene.</param>
    /// <returns>The SceneAsset if found, otherwise null.</returns>
    protected SceneAsset GetSceneObject(string sceneObjectName)
    {
        if (string.IsNullOrEmpty(sceneObjectName))
            return null;

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
            if (scene.path.IndexOf(sceneObjectName, StringComparison.Ordinal) != -1)
            {
                return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
            }
        }

        Debug.Log("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in the build settings.");
        return null;
    }

    /// <summary>
    /// Draws the custom property GUI for SceneObject.
    /// </summary>
    /// <param name="position">The position of the property in the inspector.</param>
    /// <param name="property">The serialized property being drawn.</param>
    /// <param name="label">The label of the property.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneObj = GetSceneObject(property.FindPropertyRelative("m_SceneName").stringValue);
        var newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
        if (newScene == null)
        {
            var prop = property.FindPropertyRelative("m_SceneName");
            prop.stringValue = "";
        }
        else
        {
            if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue)
            {
                var scnObj = GetSceneObject(newScene.name);
                if (scnObj == null)
                {
                    Debug.LogWarning("The scene " + newScene.name + " cannot be used. To use this scene add it to the build settings for the project.");
                }
                else
                {
                    var prop = property.FindPropertyRelative("m_SceneName");
                    prop.stringValue = newScene.name;
                }
            }
        }
    }
}
#endif