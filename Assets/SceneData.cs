using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Scene Data", menuName = "ProjectHotline/Scene Data")]
public class SceneData : ScriptableObject
{
    public List<SScene> sceneObjects;

    public void ChangeIsLoaded(SScene scene, bool b)
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            if (sceneObjects[i].Equals(scene))
            {
                sceneObjects[i].isLoaded = b;
            }
        }
    }
}

[Serializable]
public struct SScene
{
    public SceneObject sceneObject;
    public bool isInitialyLoaded;

    public bool isLoaded;
}
