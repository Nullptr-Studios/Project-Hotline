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
}

[Serializable]
public struct SScene
{
    public SceneObject sceneObject;
    public SceneObject EnemyScene;
    public bool isInitialyLoaded;
    public bool isInitialyActive;
    
    public SCameraVariables cameraBehaviour;
}
