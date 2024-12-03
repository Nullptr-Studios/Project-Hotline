using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Manages resources such as bullet trails, blood, and corpses using object pooling.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public Material RetroMaterial;
    private static Material _retroMaterial;
    
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
    public GameObject _BloodManager;

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
    private static ObjectPool<GameObject> _BloodManagerPool;

    /// <summary>
    /// Gets the object pool for muzzle VFX.
    /// </summary>
    /// <returns>The object pool for muzzle VFX.</returns>
    public static ObjectPool<GameObject> GetMuzzlePool() => _MuzzleVFXPool;

    /// <summary>
    /// Gets the object pool for wallbang hits.
    /// </summary>
    /// <returns>The object pool for wallbang hits.</returns>
    public static ObjectPool<GameObject> GetWallBangHitPool() => _WallbangHitPool;

    /// <summary>
    /// Gets the object pool for wall hits.
    /// </summary>
    /// <returns>The object pool for wall hits.</returns>
    public static ObjectPool<GameObject> GetWallHitPool() => _WallHitPool;

    /// <summary>
    /// Gets the object pool for glass hits.
    /// </summary>
    /// <returns>The object pool for glass hits.</returns>
    public static ObjectPool<GameObject> GetGlassHitPool() => _GlassHitPool;

    /// <summary>
    /// Gets the object pool for blood managers.
    /// </summary>
    /// <returns>The object pool for blood managers.</returns>
    public static ObjectPool<GameObject> GetBloodManagerPool() => _BloodManagerPool;

    /// <summary>
    /// Gets the object pool for blood.
    /// </summary>
    /// <returns>The object pool for blood.</returns>
    public static ObjectPool<GameObject> GetBloodPool() => _bloodPool;

    /// <summary>
    /// Gets the object pool for bullet trails.
    /// </summary>
    /// <returns>The object pool for bullet trails.</returns>
    public static ObjectPool<TrailRenderer> GetBulletTrailPool() => _bulletTrailPool;

    /// <summary>
    /// Gets the object pool for corpses.
    /// </summary>
    /// <returns>The object pool for corpses.</returns>
    public static ObjectPool<GameObject> GetCorpsePool() => _corpsePool;

    /// <summary>
    /// Gets the object pool for civilian corpses.
    /// </summary>
    /// <returns>The object pool for civilian corpses.</returns>
    public static ObjectPool<GameObject> GetCivilianCorpsePool() => _civilianCorpsePool;

    /// <summary>
    /// Gets the bullet trail configuration.
    /// </summary>
    /// <returns>The bullet trail configuration.</returns>
    public static BulletTrailConfig GetBulletTrailConfig() => _bulletTrailConfig;

    /// <summary>
    /// Gets the corpse configuration.
    /// </summary>
    /// <returns>The corpse configuration.</returns>
    public static CorpseConfig GetCorpseConfig() => _corpseConfig;

    /// <summary>
    /// Initializes the ResourceManager and sets up object pools.
    /// </summary>
    void Awake()
    {
        // Static configurations
        _bulletTrailConfig = bulletTrailConfig;
        _corpseConfig = corpseConfig;
        
        _retroMaterial = RetroMaterial;

        // Initialize object pools
        _bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        _bloodPool = new ObjectPool<GameObject>(CreateBlood);
        _corpsePool = new ObjectPool<GameObject>(() => CreateCorpse(false));
        _civilianCorpsePool = new ObjectPool<GameObject>(() => CreateCorpse(true));
        _WallHitPool = new ObjectPool<GameObject>(() => Instantiate(_WallHit, transform, true));
        _GlassHitPool = new ObjectPool<GameObject>(() => Instantiate(_GlassHit, transform, true));
        _WallbangHitPool = new ObjectPool<GameObject>(() => Instantiate(_WallbangHit, transform, true));
        _MuzzleVFXPool = new ObjectPool<GameObject>(() => Instantiate(_MuzzleVFX, transform, true));
        _BloodManagerPool = new ObjectPool<GameObject>(() => Instantiate(_BloodManager, transform, true));
    }

    public static void ChangeStaticEffect(bool _static)
    {
        if(_static)
            _retroMaterial.EnableKeyword("_Scrolling_Static");
        else
            _retroMaterial.DisableKeyword("_Scrolling_Static");
    }

    /// <summary>
    /// Creates a new corpse instance.
    /// </summary>
    /// <param name="isCivilian">Indicates if the corpse is civilian.</param>
    /// <returns>A new corpse instance.</returns>
    private GameObject CreateCorpse(bool isCivilian)
    {
        Transform parentTransform = transform;
        GameObject instance = new GameObject("Corpse")
        {
            transform = { parent = parentTransform },
            tag = "Corpse",
            layer = 10
        };

        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        List<Sprite> sprites = isCivilian ? corpseConfig.CivilianSprites : corpseConfig.Sprites;
        spr.sprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
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
        GameObject instance = new GameObject("BloodSplatter")
        {
            tag = "Blood",
            transform = { parent = transform, eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360)) }
        };

        SpriteRenderer spr = instance.AddComponent<SpriteRenderer>();
        spr.sprite = bloodConfig.Sprites[UnityEngine.Random.Range(0, bloodConfig.Sprites.Count)];
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
        GameObject instance = new GameObject("Bullet Trail")
        {
            transform = { parent = transform }
        };

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