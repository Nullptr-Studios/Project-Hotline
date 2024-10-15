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
    private float _timer = 0.0f;

    private void Start()
    {
        finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        transform.localScale = new Vector3(0, 0, 1);
    }

    void Update()
    {
        if (transform.localScale.x < .95f)
        {
            transform.localScale = new Vector3(math.lerp(transform.localScale.x, 1, finalLerpMagnitude * Time.deltaTime),
                math.lerp(transform.localScale.y, 1, finalLerpMagnitude * Time.deltaTime), 1);
        }
        else
        {
            Destroy(this);
        }
    }
}
