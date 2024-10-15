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

    private void Start()
    {
        finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        transform.localScale = new Vector3(.95f, .95f, 1);
    }

    void Update()
    {
        if (transform.localScale.x > .05f)
        {
            transform.localScale = new Vector3(math.lerp(transform.localScale.x, 0, finalLerpMagnitude * Time.deltaTime),
                math.lerp(transform.localScale.y, 0, finalLerpMagnitude * Time.deltaTime), 1);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(this);
            ResourceManager.GetBloodPool().Release(gameObject);
        }
    }
}
