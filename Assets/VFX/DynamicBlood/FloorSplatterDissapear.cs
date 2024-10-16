using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FloorSplatterDisapear : MonoBehaviour
{

    public float lerpMagnitudeMax = 2;
    public float lerpMagnitudeMin = 1;

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

        if (scale.x < .05f)
        {
            gameObject.SetActive(false);
            Destroy(this);
            ResourceManager.GetBloodPool().Release(gameObject);
            return;
        }
        
        float finalLerpMagnitudeCached = finalLerpMagnitude * Time.deltaTime;
        float lerpVal = math.lerp(scale.x, 0, finalLerpMagnitudeCached); 
        
        scale = new Vector3(lerpVal, lerpVal, 1);

        _trans.localScale = scale;
    }
}
