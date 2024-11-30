using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextEffect : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> Scoretext;
    
    public AnimationCurve curve;
    
    private Coroutine _scoreCoroutine;
    private Coroutine _scaleCoroutine;
    
    private string fmt = "00000";

    
    private int score;
    private int initialScore;

    // Start is called before the first frame update
    void Start()
    {
        initialScore = 0;
        score = 0;
        ScoreManager.AddedScoreDelegate += ShowScore;
    }

    private void OnDisable()
    {
        ScoreManager.AddedScoreDelegate -= ShowScore;
    }

    private void ShowScore()
    {
        initialScore = score;
        score = ScoreManager._playerKills * (int)ScoreManager._killXP + ScoreManager._playerCivilianKills * (int)ScoreManager._killCivilianXP;
        
        if(_scoreCoroutine != null)
            StopCoroutine(_scoreCoroutine);
        if(_scaleCoroutine != null)
            StopCoroutine(_scaleCoroutine);
        
        _scoreCoroutine = StartCoroutine(scoreCoroutine());
        _scaleCoroutine = StartCoroutine(scaleCoroutine());
    }

    private IEnumerator scaleCoroutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime <= curve.GetDuration())
        {
            float font = curve.Evaluate(elapsedTime);
            foreach (var t in Scoretext)
            {
                t.fontSize = font;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator scoreCoroutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime <= .15f)
        {
            initialScore = (int)Mathf.Lerp(initialScore, score, .1f);

            foreach (var t in Scoretext)
            {
                t.text = initialScore.ToString(fmt);
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        foreach (var t in Scoretext)
        {
            t.text = score.ToString(fmt);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int k = 0; k < Scoretext.Count; k++)
        {
            //Update mesh
            Scoretext[k].ForceMeshUpdate();
            Mesh mesh = Scoretext[k].mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 offset = Wobble(Time.time + i);

                vertices[i] += offset;
            }

            mesh.vertices = vertices;
            
            Scoretext[k].canvasRenderer.SetMesh(mesh);

            //Update rotation
            Scoretext[k].transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin((Time.time - k * .25f) * 2.5f) * 2.5f);
        }
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f) * 1f, Mathf.Cos(time * 2.5f) * 1f);
    }
}
