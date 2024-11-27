using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyModeSchizo : MonoBehaviour
{
    [NonSerialized] bool IsBabyMode = false;

    private void Awake()
    {
        IsBabyMode = ToolBox.Serialization.DataSerializer.Load<bool>(SaveKeywords.BabyMode);

        if (IsBabyMode)
        {
            gameObject.SetActive(false);
        }
    }

    private const string README = "fuck digipen for making us do this";
}
