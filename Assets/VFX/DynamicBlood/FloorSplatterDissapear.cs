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
        float finalLerpMagnitudeCached = finalLerpMagnitude * Time.deltaTime;

        if (scale.x > .05f)
        {
            scale = new Vector3(math.lerp(scale.x, 0, finalLerpMagnitudeCached),
                math.lerp(scale.y, 0, finalLerpMagnitudeCached), 1);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(this);
            ResourceManager.GetBloodPool().Release(gameObject);
            return;
        }

        _trans.localScale = scale;
    }
}
