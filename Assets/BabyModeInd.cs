using System;
using System.Collections;
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class BabyModeInd : MonoBehaviour
{
    private Image _spr;
    private void Awake()
    {
        _spr = GetComponent<Image>();
    }

    private void OnEnable()
    {
        UpdateEnabled();
    }
    
    public void ChangeBabyMode()
    {
        DataSerializer.Save(SaveKeywords.BabyMode, !DataSerializer.Load<bool>(SaveKeywords.BabyMode));
        UpdateEnabled();
    }

    public void UpdateEnabled()
    {
        if (DataSerializer.Load<bool>(SaveKeywords.BabyMode))
        {
            _spr.color = new Color(0, 1, 0, 1);    
        }
        else
        {
            _spr.color = new Color(1, 0, 0, 1);    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
