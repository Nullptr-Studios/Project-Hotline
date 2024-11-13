using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Manages resources such as bullet trails, blood, and corpses using object pooling.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    /// <summary>
    /// Configuration for bullet trails.
    /// </summary>
    public BulletTrailConfig bulletTrailConfig;

    /// <summary>
    /// Configuration for blood.
    /// </summary>
    public BloodConfig bloodConfig;

    /// <summary>
    /// Configuration for corpses.
    /// </summary>
    public CorpseConfig corpseConfig;

    public GameObject _WallHit;
    public GameObject _GlassHit;
    public GameObject _WallbangHit;

    public GameObject _MuzzleVFX;
    // Static fields for shared configurations and pools
    private static BulletTrailConfig _bulletTrailConfig;
    private static CorpseConfig _corpseConfig;

    // Object pools for bullet trails, blood, and corpses
    private static ObjectPool<TrailRenderer> _bulletTrailPool;
    private static ObjectPool<GameObject> _bloodPool;
    private static ObjectPool<GameObject> _corpsePool;
    private static ObjectPool<GameObject> _civilianCorpsePool;

    private static ObjectPool<GameObject> _WallHitPool;
    private static ObjectPool<GameObject> _GlassHitPool;
    private static ObjectPool<GameObject> _WallbangHitPool;

    private static ObjectPool<GameObject> _MuzzleVFXPool;

    public static ObjectPool<GameObject> GetMuzzlePool()
    {
        return _MuzzleVFXPool;
    }

    public static ObjectPool<GameObject> GetWallBangHitPool()
    {
        return _WallbangHitPool;
    }

    public static ObjectPool<GameObject> GetWallHitPool()
    {
        return _WallHitPool;
    }

    public static ObjectPool<GameObject> GetGlassHitPool()
    {
        return _GlassHitPool;
    }

    /// <summary>
    /// Gets the object pool for blood instances.
    /// </summary>
    /// <returns>The object pool for blood instances.</returns>
    public static ObjectPool<GameObject> GetBloodPool()
    {
        return _bloodPool;
    }

    /// <summary>
    /// Gets the object pool for bullet trail instances.
    /// </summary>
    /// <returns>The object pool for bullet trail instances.</returns>
    public static ObjectPool<TrailRenderer> GetBulletTrailPool()
    {
        return _bulletTrailPool;
    }

    /// <summary>
    /// Gets the object pool for corpse instances.
    /// </summary>
    /// <returns>The object pool for corpse instances.</returns>
    public static ObjectPool<GameObject> GetCorpsePool()
    {
        return _corpsePool;
    }
    
    public static ObjectPool<GameObject> GetCivilianCorpsePool()
    {
        return _civilianCorpsePool;
    }

    /// <summary>
    /// Gets the configuration for bullet trails.
    /// </summary>
    /// <returns>The configuration for bullet trails.</returns>
    public static BulletTrailConfig GetBulletTrailConfig()
    {
        return _bulletTrailConfig;
    }

    /// <summary>
    /// Gets the configuration for corpses.
    /// </summary>
    /// <returns>The configuration for corpses.</returns>
    public static CorpseConfig GetCorpseConfig()
    {
        return _corpseConfig;
    }

    /// <summary>
    /// Initializes the ResourceManager and sets up object pools.
    /// </summary>
    void Awake()
    {
        // Static configurations
        _bulletTrailConfig = bulletTrailConfig;
        _corpseConfig = corpseConfig;

        // Initialize object pools
        _bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        _bloodPool = new ObjectPool<GameObject>(CreateBlood);
        _corpsePool = new ObjectPool<GameObject>(CreateCorpse);
        _civilianCorpsePool = new ObjectPool<GameObject>(CreateCorpseCivilian);

        _WallHitPool = new ObjectPool<GameObject>(CreateWallHit);
        _GlassHitPool = new ObjectPool<GameObject>(CreateWallHit);
        _WallbangHitPool = new ObjectPool<GameObject>(CreateWallbangHit);

        _MuzzleVFXPool = new ObjectPool<GameObject>(CreateMuzzleVFX);
    }

    private GameObject CreateWallHit()
    {
        GameObject obj = Instantiate(_WallHit);
        obj.transform.parent = transform;
        return obj;
    }

    private GameObject CreateGlassHit() 
    {
        GameObject obj = Instantiate(_GlassHit);
        obj.transform.parent = transform;
        return obj;
    }

    private GameObject CreateWallbangHit()
    {
        GameObject obj = Instantiate(_WallbangHit);
        obj.transform.parent = transform;
        return obj;
    }

    private GameObject CreateMuzzleVFX()
    {
        GameObject obj = Instantiate(_MuzzleVFX);
        obj.transform.parent = transform;
        return obj;
    }

    /// <summary>
    /// Creates a new corpse instance.
    /// </summary>
    /// <returns>A new corpse instance.</returns>
    private GameObject CreateCorpse()
    {
        GameObject instance = new GameObject("Corpse");
        instance.transform.parent = transform;
        instance.tag = "Corpse";

        instance.layer = 10;

        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        spr.sprite = corpseConfig.Sprites[UnityEngine.Random.Range(0, corpseConfig.Sprites.Count - 1)];
        spr.material = corpseConfig.Material;
        spr.sortingOrder = -1;
        return instance;
    }
    
    private GameObject CreateCorpseCivilian()
    {
        GameObject instance = new GameObject("Corpse");
        instance.transform.parent = transform;
        instance.tag = "Corpse";

        instance.layer = 10;

        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        spr.sprite = corpseConfig.CivilianSprites[UnityEngine.Random.Range(0, corpseConfig.CivilianSprites.Count - 1)];
        spr.material = corpseConfig.Material;
        spr.sortingOrder = -1;
        return instance;
    }

    /// <summary>
    /// Creates a new blood instance.
    /// </summary>
    /// <returns>A new blood instance.</returns>
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

    /// <summary>
    /// Creates a new bullet trail instance.
    /// </summary>
    /// <returns>A new bullet trail instance.</returns>
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