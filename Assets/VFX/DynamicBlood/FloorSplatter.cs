using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FloorSplatter : MonoBehaviour
{

    public float lerpMagnitudeMax = 2;
    public float lerpMagnitudeMin = 4;

    private float _finalLerpMagnitude;

    private Transform _trans;

    private void Start()
    {
        _finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        _trans = transform;
        _trans.localScale = new Vector3(0, 0, 0);
    }

    void Update()
    {
        Vector3 scale = _trans.localScale;

        if (scale.x > .95f)
        {
            Destroy(this);
            return;
        }
        
        float finalLerpMagnitudeCached = _finalLerpMagnitude * Time.deltaTime;

        //since scale is linear we can do this optimization and save 1 lerp
        float lerpVal = math.lerp(scale.x, 1, finalLerpMagnitudeCached);
        
        scale = new Vector3(lerpVal, lerpVal, 1);

        _trans.localScale = scale;
    }
}
