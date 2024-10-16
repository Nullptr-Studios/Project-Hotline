using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FloorSplatter : MonoBehaviour
{

    public float lerpMagnitudeMax = 2;
    public float lerpMagnitudeMin = 4;

    private float finalLerpMagnitude;

    private Transform _trans;

    private void Start()
    {
        finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        _trans = transform;
    }

    void Update()
    {
        Vector3 scale = _trans.localScale;
        float finalLerpMagnitudeCached = finalLerpMagnitude * Time.deltaTime;

        if (scale.x < .95f)
        {
            scale = new Vector3(math.lerp(scale.x, 1, finalLerpMagnitudeCached),
                math.lerp(scale.y, 1, finalLerpMagnitudeCached), 1);
        }
        else
        {
            Destroy(this);
            return;
        }

        _trans.localScale = scale;
    }
}
