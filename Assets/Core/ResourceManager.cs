using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceManager : MonoBehaviour
{
    public BulletTrailConfig bulletTrailConfig;
    public BloodConfig bloodConfig;
    public CorpseConfig corpseConfig;
    
    //Static shit
    private static BulletTrailConfig _bulletTrailConfig;
    private static CorpseConfig _corpseConfig;

    private static PlayerIA _playerIa;
    
    //This is done here so enemies can share the same pool
    private static ObjectPool<TrailRenderer> _bulletTrailPool;
    private static ObjectPool<GameObject> _bloodPool;
    private static ObjectPool<GameObject> _corpsePool;


    public static ObjectPool<GameObject> GetBloodPool()
    {
        return _bloodPool;
    }
    public static ObjectPool<TrailRenderer> GetBulletTrailPool()
    {
        return _bulletTrailPool;
    }
    public static ObjectPool<GameObject> GetCorpsePool()
    {
        return _corpsePool;
    }
    
    public static BulletTrailConfig GetBulletTrailConfig()
    { 
        return _bulletTrailConfig;
    }
    public static CorpseConfig GetCorpseConfig()
    {
        return _corpseConfig;
    }

    public static PlayerIA GetPlayerIA()
    {
        if (_playerIa == null)
            _playerIa = new PlayerIA();
        
        return _playerIa;
    }
    // Start is called before the first frame update
    void Awake()
    {
        //Static shit
        _bulletTrailConfig = bulletTrailConfig;
        _corpseConfig = corpseConfig;
        //_playerIa = new PlayerIA();
        
        //initialize trail pooling
        _bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        //blood
        _bloodPool = new ObjectPool<GameObject>(CreateBlood);

        _corpsePool = new ObjectPool<GameObject>(CreateCorpse);
    }

    private GameObject CreateCorpse()
    {
        GameObject instance = new GameObject("Corpse");
        instance.transform.parent = transform;
        
        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        spr.sprite = corpseConfig.Sprites[UnityEngine.Random.Range(0, corpseConfig.Sprites.Count - 1)];
        spr.material = corpseConfig.Material;
        spr.sortingOrder = -1;
        return instance;
    }

    private GameObject CreateBlood()
    {
        GameObject instance = new GameObject("BloodSplatter");
        instance.tag = "Blood";
        instance.transform.parent = transform;
        instance.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));

        
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
