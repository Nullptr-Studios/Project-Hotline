using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// Represents a fire weapon with various functionalities such as firing, reloading, and applying effects.
/// Inherits from the Weapon class.
/// </summary>
public class FireWeapon : Weapon
{
    [Header("Fire Weapon")]
    public FireWeaponData fireWeaponData;

    // Gun muzzle will be used for FX
    public Transform gunMuzzle;
    public MuzzleLightController muzzleLightController;
    // The transform where the ray casts will be emitted
    public Transform dispersionTransform;

    private int _currentAmmo;
    private bool _isReloading;

    // Fire rate global variables
    private float _fireRateTimer;
    private float _fireRateCurveTimer;

    private bool _wantsToFire;
    private bool _canFire = true;

    // Dispersion global variables
    private float _currentDispersion;
    private float _dispersionCurveTimer;

    private float _currentTimeToFire;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
    [SerializeField] private float gizmosDuration = .5f;
#endif

    private Transform _transform;

    /// <summary>
    /// Initializes the FireWeapon instance.
    /// </summary>
    private void Awake()
    {
        weaponType = EWeaponType.Fire;
        _transform = transform;
        _currentAmmo = fireWeaponData.maxAmmo;
    }

    /// <summary>
    /// Gets the weapon sprite ID.
    /// </summary>
    /// <returns>The weapon sprite ID.</returns>
    public override int GetWeaponSpriteID() => fireWeaponData.SpriteAnimID;

    /// <summary>
    /// Gets the number of uses left for the weapon.
    /// </summary>
    /// <returns>The number of uses left.</returns>
    public override int UsesLeft() => _currentAmmo;

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    /// <returns>True if automatic, false otherwise.</returns>
    public override bool IsAutomatic() => fireWeaponData.automatic;

    /// <summary>
    /// Gets the reload time of the weapon.
    /// </summary>
    /// <returns>The reload time.</returns>
    public override float ReloadTime() => fireWeaponData.reloadTime;

    /// <summary>
    /// Gets the time between uses of the weapon.
    /// </summary>
    /// <returns>The time between uses.</returns>
    public override float TimeBetweenUses() => _currentTimeToFire;

    /// <summary>
    /// Gets the maximum number of uses for the weapon.
    /// </summary>
    /// <returns>The maximum number of uses.</returns>
    public override int MaxUses() => fireWeaponData.maxAmmo;

    /// <summary>
    /// Handles the use action of the weapon.
    /// </summary>
    /// <param name="pressed">True if the use action is pressed, false otherwise.</param>
    public override void Use(bool pressed)
    {
        if (_currentAmmo < 0 && isPlayer)
        {
            _wantsToFire = false;
            return;
        }
        
        
            
        _wantsToFire = pressed;
        if (pressed)
        {
            if (fireWeaponData.useFireRateCurve)
            {
                _fireRateTimer = fireWeaponData.fireRateCurve.Evaluate(0);
                _currentTimeToFire = _fireRateTimer;
            }
        }
        else
        {
            _fireRateTimer = 0;
            _fireRateCurveTimer = 0;
            _dispersionCurveTimer = 0;
        }
    }

    /// <summary>
    /// Initializes the FireWeapon instance.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        if (fireWeaponData)
        {
            _currentAmmo = fireWeaponData.maxAmmo;
        }
        else
        {
#if UNITY_EDITOR
            if (log)
                Debug.LogError($"FireWeapon Error: {gameObject.name} does not have fireWeaponData assigned!");
#endif
        }
        if (!gunMuzzle)
        {
#if UNITY_EDITOR
            if (log)
                Debug.LogError($"FireWeapon Error: {gameObject.name} does not have gunMuzzle assigned!");
#endif
        }
    }

    /// <summary>
    /// Performs bullet visual effects based on the hit layer.
    /// </summary>
    /// <param name="layer">The layer of the hit object.</param>
    /// <param name="hit">The RaycastHit2D object containing hit information.</param>
    private void DoBulletVFX(int layer, RaycastHit2D hit)
    {
        GameObject obj = layer switch
        {
            6 => ResourceManager.GetWallHitPool().Get(),
            7 => ResourceManager.GetGlassHitPool().Get(),
            8 => ResourceManager.GetWallBangHitPool().Get(),
            _ => null
        };

        if (obj != null)
        {
            obj.SetActive(true);
            obj.transform.position = hit.point;
            obj.transform.LookAt(hit.point + hit.normal);
        }
    }

    /// <summary>
    /// Picks up the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public override void Pickup(Transform weaponHolder)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        base.Pickup(weaponHolder);
    }

    /// <summary>
    /// Throws the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public override void Throw(Vector2 forwardVector)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        base.Throw(forwardVector);
    }

    /// <summary>
    /// Drops the weapon.
    /// </summary>
    public override void Drop()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        base.Drop();
    }

    /// <summary>
    /// Fires the weapon in the specified direction.
    /// </summary>
    /// <param name="fireDir">The transform representing the fire direction.</param>
    private void Fire(Transform fireDir)
    {
        var contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = ~LayerMask.GetMask("HalfHeight")
        };

        List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
        int amountHits = Physics2D.Raycast(fireDir.position, fireDir.right, contactFilter, rayHitList);

        if (amountHits == 0)
        {
            StartCoroutine(PlayTrail(fireDir.position, fireDir.position + fireDir.right * ResourceManager.GetBulletTrailConfig().MissDistance, new RaycastHit2D()));
            return;
        }

        Vector2 lastHitPos = fireDir.position;
        int currentPenetration = 0;

        foreach (var hit2D in rayHitList)
        {
            int layer = hit2D.transform.gameObject.layer;

#if UNITY_EDITOR
            if (drawGizmos)
            {
                float segmentDistance = Vector3.Distance(lastHitPos, hit2D.point);
                Debug.DrawRay(lastHitPos, fireDir.right * segmentDistance, GetRayColorDebug(layer), gizmosDuration);
                lastHitPos = hit2D.point;
            }
#endif
            if (hit2D.transform.TryGetComponent(out IDamageable damageableInterface))
                damageableInterface.DoDamage(1, fireDir.right, hit2D.point, weaponType);

            if (layer == 3) continue;

            if (layer == 6)
            {
                StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                DoBulletVFX(layer, hit2D);
                break;
            }

            if (layer == 7 || layer == 8 || layer == 13)
            {
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                DoBulletVFX(layer == 7 ? layer : 8, hit2D);
                currentPenetration++;
                continue;
            }

            if (layer == 9)
            {
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                currentPenetration++;
                continue;
            }

            StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
            DoBulletVFX(6, hit2D);
            break;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Gets the debug color for the ray based on the layer.
    /// </summary>
    /// <param name="layer">The layer of the hit object.</param>
    /// <returns>The color for the debug ray.</returns>
    private Color GetRayColorDebug(int layer) => layer switch
    {
        6 => Color.red,
        7 or 8 => Color.yellow,
        9 => Color.green,
        _ => Color.white
    };
#endif

    /// <summary>
    /// Applies random dispersion to the weapon's fire direction.
    /// </summary>
    private void RandomDispersion()
    {
        dispersionTransform.localEulerAngles = fireWeaponData.useDispersion
            ? new Vector3(0, 0, Random.Range(-_currentDispersion, _currentDispersion))
            : Vector3.zero;
    }

    /// <summary>
    /// Implements the firing logic of the weapon.
    /// </summary>
    private void FireImplementation()
    {
        if (fireWeaponData.fireType == FireType.Simple)
        {
            RandomDispersion();
            Fire(dispersionTransform);
        }
        else if (fireWeaponData.fireType == FireType.Multiple)
        {
            for (int i = 0; i < fireWeaponData.fireCastsAmount; i++)
            {
                RandomDispersion();
                Fire(dispersionTransform);
            }
        }

        GameObject mVFX = ResourceManager.GetMuzzlePool().Get();
        mVFX.SetActive(true);
        mVFX.transform.position = gunMuzzle.position;
        mVFX.transform.forward = gunMuzzle.right;

        muzzleLightController.ActivateLight(fireWeaponData.sfxMuzzleFlashCurve);
        FMODUnity.RuntimeManager.PlayOneShot(fireWeaponData.fireSound, _transform.position);

        if (isPlayer)
        {
           HearEnemy();
           _currentAmmo--; 
        }
            
    }

    /// <summary>
    /// Gets the hearing range of the weapon.
    /// </summary>
    /// <returns>The hearing range.</returns>
    public override float GetHearingRange() => fireWeaponData.enemyHearingDistance;

    /// <summary>
    /// Notifies enemies within hearing range of the weapon's fire.
    /// </summary>
    private void HearEnemy()
    {
        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy")
        };

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        if (Physics2D.CapsuleCast(_transform.position, new Vector2(fireWeaponData.enemyHearingDistance, fireWeaponData.enemyHearingDistance), CapsuleDirection2D.Vertical, 0, Vector2.zero, filter, hits) > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent(out AISensor aiSensor))
                {
                    aiSensor.HeardPlayer(_transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Updates the can fire status.
    /// </summary>
    private void UpdateCanFire() => _canFire = true;

    /// <summary>
    /// Finishes the reloading process.
    /// </summary>
    private void FinishReloading()
    {
#if UNITY_EDITOR
        if (log)
            Debug.Log("Reloaded!!");
#endif
        _isReloading = false;
        _currentAmmo = fireWeaponData.maxAmmo;

        _fireRateTimer = 0;
        _fireRateCurveTimer = 0;
        _dispersionCurveTimer = 0;
    }

    /// <summary>
    /// Plays the bullet trail effect.
    /// </summary>
    /// <param name="startPoint">The start point of the trail.</param>
    /// <param name="endPoint">The end point of the trail.</param>
    /// <param name="hit">The RaycastHit2D object containing hit information.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit2D hit)
    {
        TrailRenderer instance = ResourceManager.GetBulletTrailPool().Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= ResourceManager.GetBulletTrailConfig().SimulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPoint;
        yield return new WaitForSeconds(ResourceManager.GetBulletTrailConfig().Duration);
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        ResourceManager.GetBulletTrailPool().Release(instance);
    }

    /// <summary>
    /// Updates the weapon state.
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (_wantsToFire)
        {
            if (_currentAmmo > 0 && isPlayer)
            {
                FireLogic();
                return;
            } 
            
            if (!isPlayer)
            {
                FireLogic();
            }
        }
    }

    private void FireLogic()
    {
        if (fireWeaponData.automatic)
        {
            if (fireWeaponData.useFireRateCurve)
            {
                _currentTimeToFire = fireWeaponData.fireRateCurve.Evaluate(_fireRateCurveTimer);

                if (_fireRateTimer >= _currentTimeToFire)
                {
                    _fireRateTimer = 0;
                    UpdateCanFire();
                }

                _fireRateTimer += Time.deltaTime;

                if (_fireRateCurveTimer < fireWeaponData.fireRateCurve.GetDuration())
                    _fireRateCurveTimer += Time.deltaTime;
            }

            if (fireWeaponData.useDispersionCurve)
            {
                _currentDispersion = fireWeaponData.bulletDispersionCurve.Evaluate(_dispersionCurveTimer);

                if (_dispersionCurveTimer < fireWeaponData.bulletDispersionCurve.GetDuration())
                    _dispersionCurveTimer += Time.deltaTime;
            }
            else
            {
                _currentDispersion = fireWeaponData.maxDispersionAngle;
            }

            if (_canFire)
            {
                _canFire = false;
                FireImplementation();

                if (!fireWeaponData.useFireRateCurve)
                {
                    _fireRateCurveTimer = fireWeaponData.finalFireRate;
                    Invoke(nameof(UpdateCanFire), fireWeaponData.finalFireRate);
                }
            }
        }
        else
        {
            if (_canFire)
            {
                _currentDispersion = fireWeaponData.maxDispersionAngle;
                _currentTimeToFire = fireWeaponData.finalFireRate;

                Invoke(nameof(UpdateCanFire), _currentTimeToFire);
                FireImplementation();

                _canFire = false;
                _wantsToFire = false;
            }
        }
    }
}