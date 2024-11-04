using UnityEngine;
using ToolBox.Serialization;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SaveSystem : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(DataSerializer.Load<int>(SaveKeywords.LevelScore));
        Debug.Log(DataSerializer.Load<bool>(SaveKeywords.LevelPassed));
    }
}

public class SaveKeywords
{
    public const string LevelScore = "LevelScore";
    public const string LevelPassed = "LevelPassed";
    public const string TestValue = "TestValue";
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveSystem))]
public class SaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var saveDataUtilities = (SaveSystem)target;
        if (saveDataUtilities == null) return;

        Undo.RecordObject(saveDataUtilities, "SaveDataUtilities");

        if (GUILayout.Button("Delete All Saved Data"))
        {
            DataSerializer.DeleteAll();
        }
    }
}
#endif
