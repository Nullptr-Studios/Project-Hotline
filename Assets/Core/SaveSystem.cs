//To use this save system, put DataSerializer.Save(SaveKeywords.KeyWordBeingSaved, variable with the value it has)
//To load it: DataSerializer.Load<List<VariableType>>(SaveKeywords.KeyWordBeingLoaded)

using UnityEngine;
using ToolBox.Serialization;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SaveSystem : MonoBehaviour
{
    
}
/// <summary>
/// Saves different parameters on the game as strings that will be assigned a value later on
/// </summary>
public class SaveKeywords
{
    public const string LevelScore = "LevelScore";
    public const string LevelPassed = "LevelPassed";
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
