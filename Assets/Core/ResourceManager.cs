using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceManager : MonoBehaviour
{
    public BulletTrailConfig bulletTrailConfig;
    public BloodConfig bloodConfig;
    
    //Static shit
    private static BulletTrailConfig _bulletTrailConfig;
    //This is done here so enemies can share the same pool
    private static ObjectPool<TrailRenderer> _bulletTrailPool;
    private static ObjectPool<GameObject> _bloodPool;


    public static ObjectPool<GameObject> GetBloodPool()
    {
        return _bloodPool;
    }
    
    public static BulletTrailConfig GetBulletTrailConfig()
    {
        return _bulletTrailConfig;
    }
    public static ObjectPool<TrailRenderer> GetBulletTrailPool()
    {
        return _bulletTrailPool;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _bulletTrailConfig = bulletTrailConfig;
        
        //initialize trail pooling
        _bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        //blood
        _bloodPool = new ObjectPool<GameObject>(CreateBlood);
    }

    private GameObject CreateBlood()
    {
        GameObject instance = new GameObject("BloodSplatter");
        instance.tag = "Blood";
        instance.transform.parent = transform;
        
        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        spr.sprite = bloodConfig.Sprites[UnityEngine.Random.Range(0, bloodConfig.Sprites.Count - 1)];
        spr.color = bloodConfig.Color;
        spr.material = bloodConfig.Material;
        return instance;
    }
    
    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.parent = transform;

        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = _bulletTrailConfig.Color;
        trail.material = _bulletTrailConfig.Material;
        trail.widthCurve = _bulletTrailConfig.WidthCurve;
        trail.time = _bulletTrailConfig.Duration;
        trail.minVertexDistance = _bulletTrailConfig.MinVertexDistance;

        trail.sortingOrder = 1;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

}
