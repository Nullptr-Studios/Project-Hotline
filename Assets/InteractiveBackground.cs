using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBackground : MonoBehaviour
{
    public AnimationCurve PulseCurve;
    public AnimationCurve ColorCurve;
    
    public float startingHue;
    
    private static readonly int ColorA = Shader.PropertyToID("_ColorA");
    private static readonly int Size = Shader.PropertyToID("_Size");

    private Material _material;
    
    
    private Coroutine _pulseCoroutine;
    private Coroutine _colorCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
        //_material.SetColor(ColorA, Random.ColorHSV());
        
        MusicManager.OnBar += OnBar;
        MusicManager.OnBeat += OnBeat;
    }
    
    private IEnumerator ChangeColor()
    {
        var time = 0f;
        Color initialC = _material.GetColor(ColorA);

        startingHue += .05f;
        if(startingHue > 1)
            startingHue = startingHue - 1;
        
        Color toColor = Color.HSVToRGB(startingHue, .5f, 1);
        while (time < ColorCurve.GetDuration())
        {
            time += Time.deltaTime;
            _material.SetColor(ColorA, Color.Lerp(initialC, toColor, ColorCurve.Evaluate(time)));
            yield return null;
        }
        
        _material.SetColor(ColorA, toColor);
    }
    
    private IEnumerator Pulse()
    {
        var time = 0f;
        while (time < PulseCurve.GetDuration())
        {
            time += Time.deltaTime;
            _material.SetFloat(Size, PulseCurve.Evaluate(time));
            yield return null;
        }
        
        _material.SetFloat(Size, PulseCurve.Evaluate(PulseCurve.GetDuration()));
    }
    
    void OnDisable()
    {
        MusicManager.OnBar -= OnBar;
        MusicManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {
        if(_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);
        
        _pulseCoroutine = StartCoroutine(Pulse());
    }

    private void OnBar()
    {
        if(_colorCoroutine != null)
            StopCoroutine(_colorCoroutine);
        
        _colorCoroutine = StartCoroutine(ChangeColor());
    }
    
}
