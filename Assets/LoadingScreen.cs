using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public delegate void FinalizedLoading();
    public static FinalizedLoading OnFinalizedLoading;
    
    public AnimationCurve curve;
    private float _timer = 0;
    
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingText2;
    
    private string _sceneName;
    private SceneData _sceneData;
    
    string t = "";

    private float timem = 0.1f;
    
    private bool finished = false;
    
    public GameObject activate;
    
    [Header("Act Popup")]
    public bool hasActPopup = false;
    public GameObject actPopup;
    
    
    private bool hasUnderscore = true;

    private void Awake()
    {
        activate.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        finished = false;
        timem = .1f;
        _timer = 0;
        
        InvokeRepeating(nameof(CMDPulse), 0.5f, 0.5f);
        
        LoadText();
    }
    
    private void LoadText()
    {
        loadingText.maxVisibleCharacters = 0;
        _sceneName = SceneManager.GetActiveScene().name;
        _sceneData = SceneMng._sceneData;
        
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        
        t += "X:/> Now Loading " + _sceneName + "...\n \n";
        t += "Loading sub-scenes: \n";

        foreach (var VARIABLE in _sceneData.sceneObjects)
        {
            t += VARIABLE.sceneObject + "\n";
            t+= VARIABLE.EnemyScene + "\n";
        }
        
        t += "\nNow Loading Resources: \n";

        foreach (var VARIABLE in allObjects)
        {
            if (VARIABLE.name.Contains("Mask"))
                continue;
            
            t += VARIABLE.name + "\n";
        }
        loadingText.text = t;
        StartCoroutine(text());
    }

    private IEnumerator text()
    {
        int cac = 1;
        
        for (var i = 0; i < t.Length; i += cac)
        {
            loadingText2.text = String.Format("{0} %", (int)((i / (float)t.Length) * 100));

            if (Time.deltaTime > timem)
            {
                cac = 10;
                timem = 0;
            }

            loadingText.maxVisibleCharacters += cac;
            
            yield return new WaitForSeconds(timem);
        }
        
        if(loadingText2.text != "100 %")
            loadingText2.text = "100 %";
        finished = true;
        _timer = 0;
    }
    
    private void CMDPulse()
    {
        if (hasUnderscore)
        {
            loadingText.text = loadingText.text.Remove(loadingText.text.Length - 1);
            loadingText.text += " ";
        }
        else
        {
            loadingText.text = loadingText.text.Remove(loadingText.text.Length - 1);
            loadingText.text += "_";
        }
        
        hasUnderscore = !hasUnderscore;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            if (_timer > 1)
            {
                OnFinalizedLoading?.Invoke();
                if (hasActPopup && actPopup != null)
                {
                    actPopup.SetActive(true);
                }
                
                gameObject.SetActive(false);
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
        else
        {
            timem = curve.Evaluate(_timer);

            _timer += Time.deltaTime;
        }
    }
    
}

