using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public GameObject bulletHitWallVFX;
    public GameObject bulletHitGlassVFX;
    public GameObject bulletHitWallBangVFX;

    public GameObject muzzleVFX;

    // Gun muzzle will be used for FX
    public Transform gunMuzzle;
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

    /// <summary>
    /// Gets the weapon sprite ID.
    /// </summary>
    /// <returns>The sprite ID of the weapon.</returns>
    public override int GetWeaponSpriteID()
    {
        return fireWeaponData.SpriteAnimID;
    }

    /// <summary>
    /// Gets the number of uses left for the weapon.
    /// </summary>
    /// <returns>The current ammo count.</returns>
    public override int UsesLeft()
    {
        return _currentAmmo;
    }

    /// <summary>
    /// Initializes the weapon type.
    /// </summary>
    private void Awake()
    {
        weaponType = EWeaponType.Fire;
    }

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    /// <returns>True if the weapon is automatic, otherwise false.</returns>
    public override bool IsAutomatic()
    {
        return fireWeaponData.automatic;
    }

    /// <summary>
    /// Gets the reload time of the weapon.
    /// </summary>
    /// <returns>The reload time in seconds.</returns>
    public override float ReloadTime()
    {
        return fireWeaponData.reloadTime;
    }

    /// <summary>
    /// Gets the time between uses of the weapon.
    /// </summary>
    /// <returns>The time between uses in seconds.</returns>
    public override float TimeBetweenUses()
    {
        return _currentTimeToFire;
    }

    /// <summary>
    /// Gets the maximum number of uses for the weapon.
    /// </summary>
    /// <returns>The maximum ammo count.</returns>
    public override int MaxUses()
    {
        return fireWeaponData.maxAmmo;
    }

    /// <summary>
    /// Handles the use functionality of the weapon.
    /// </summary>
    /// <param name="pressed">Indicates if the use button is pressed.</param>
    public override void Use(bool pressed)
    {
        _wantsToFire = pressed;
        if (pressed)
        {
            if (fireWeaponData.useFireRateCurve)
            {
                // So it fires as soon as it´s pressed
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
    /// Start is called before the first frame update.
    /// </summary>
    protected override void Start()
    {
        // Implement Base class functionality
        base.Start();

        if (fireWeaponData)
        {
            _currentAmmo = fireWeaponData.maxAmmo;
        }
        else
        {
#if UNITY_EDITOR
            if(log)
                Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have fireWeaponData assigned!!!!!!!!");
#endif
        }
        if (!gunMuzzle)
        {
#if UNITY_EDITOR
            if(log)
                Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have gunMuzzle assigned!!!!!!!!");
#endif
        }
    }

    /// <summary>
    /// Creates bullet hit visual effects.
    /// </summary>
    /// <param name="VFX">The visual effect to instantiate.</param>
    /// <param name="hit">The hit information from the raycast.</param>
    private void DoBulletVFX(GameObject VFX, RaycastHit2D hit)
    {
        // Do bulletHitVFX Wall
        GameObject hitVFX = Instantiate(VFX, hit.point, new Quaternion());
        hitVFX.transform.LookAt(hit.point + hit.normal);

        Destroy(hitVFX, 1);
    }

    /// <summary>
    /// Handles the pickup functionality of the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public override void Pickup(Transform weaponHolder)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        base.Pickup(weaponHolder);
    }

    /// <summary>
    /// Handles the throw functionality of the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public override void Throw(Vector2 forwardVector)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        base.Throw(forwardVector);
    }

    /// <summary>
    /// Handles the drop functionality of the weapon.
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
        // I don't like that this is hardcoded but we don't need to ignore any other layer so ig it's ok -x
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(~LayerMask.GetMask("HalfHeight"));
        contactFilter.useLayerMask = true;

        // Ray-cast2D list
        List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
        int amountHits = Physics2D.Raycast(fireDir.position, fireDir.right, contactFilter, rayHitList);

        if (amountHits == 0)
        {
            // Do trail even if we aren't hitting anything
            StartCoroutine(PlayTrail(fireDir.position, fireDir.position + fireDir.right * ResourceManager.GetBulletTrailConfig().MissDistance, new RaycastHit2D()));
            return;
        }

        // Initial Position
        Vector2 lastHitPos = fireDir.position;

        // Amount of penetrated enemies
        int currentPenetration = 0;

        foreach (var hit2D in rayHitList)
        {
            int layer = hit2D.transform.gameObject.layer;

            // Gizmos shit
#if UNITY_EDITOR
            if (drawGizmos)
            {
                float segmentDistance = Vector3.Distance(lastHitPos, hit2D.point);
                Debug.DrawRay(lastHitPos, fireDir.right * segmentDistance, GetRayColorDebug(hit2D.transform.gameObject.layer), gizmosDuration);

                lastHitPos = hit2D.point;
            }
#endif
            // Do damage to all surfaces
            if (hit2D.transform.TryGetComponent(out IDamageable damageableInterface))
                damageableInterface.DoDamage(1, fireDir.right, hit2D.point, weaponType);

            // Ignore weapon layer
            if (layer == 3)
                continue;

            if (layer == 6) // wall
            {
                // If it's a wall we want to return, no more looping

                // Do trail
                StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));

                // Bullet hit VFX
                DoBulletVFX(bulletHitWallVFX, hit2D);

                break;
            }

            if (layer == 7 || layer == 8 || layer == 13)   // wall-bang
            {
                // if bullets can penetrate continue loop

                // Calculate penetration
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    // Do trail
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                if (layer == 7) // Glass
                {
                    DoBulletVFX(bulletHitGlassVFX, hit2D);
                }else // wallbang
                {
                    DoBulletVFX(bulletHitWallBangVFX, hit2D);
                }

                currentPenetration++;
                continue;
            }

            if (layer == 9) // enemies
            {
                // Calculate penetration
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    // Do trail
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                currentPenetration++;
                continue;
            }

            // Do trail
            StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));

            DoBulletVFX(bulletHitWallVFX, hit2D);

            // We treat default objects as walls
            break;
        }

        /* There is a bug that does not render trails if there is no more colliders, won't fix since the game will take place indoors */
    }

#if UNITY_EDITOR
    /// <summary>
    /// Determines the ray color based on the layer.
    /// </summary>
    /// <param name="layer">The layer of the object hit by the ray.</param>
    /// <returns>The color of the ray for debugging purposes.</returns>
    Color GetRayColorDebug(int layer)
    {
        switch (layer)
        {
            case 6: return Color.red; // Wall

            case 7:
            case 8: return Color.yellow; // Wall-bang things

            case 9:
                return Color.green; // Enemies

            default: return Color.white; // Default color
        }
    }
#endif

    /// <summary>
    /// Calculates random dispersion for the weapon.
    /// </summary>
    private void RandomDispersion()
    {
        if (fireWeaponData.useDispersion)
        {
            float randAngle = Random.Range(-_currentDispersion, _currentDispersion);
            dispersionTransform.localEulerAngles = new Vector3(0, 0, randAngle);
        }
        else
        {
            dispersionTransform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// Handles the logic between simple and multiple types of fires.
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

        // Muzzle VFX
        GameObject mVFX = Instantiate(muzzleVFX, gunMuzzle.transform);
        mVFX.transform.forward = gunMuzzle.transform.right;
        Destroy(mVFX, 1);

        // Play fire sound
        FMODUnity.RuntimeManager.PlayOneShot(fireWeaponData.fireSound, transform.position);

        // Subtract current ammo
        _currentAmmo--;
    }

    /// <summary>
    /// Updates the can fire state. Called via Invoke.
    /// </summary>
    private void UpdateCanFire()
    {
        _canFire = true;
    }

    /// <summary>
    /// Finishes the reloading logic. Called via Invoke.
    /// </summary>
    private void FinishReloading()
    {
#if UNITY_EDITOR
        if(log)
            Debug.Log("Reloaded!!");
#endif
        _isReloading = false;
        _currentAmmo = fireWeaponData.maxAmmo;

        // Reset variables to 0
        _fireRateTimer = 0;
        _fireRateCurveTimer = 0;

        _dispersionCurveTimer = 0;

        // Play finish reload sound
        FMODUnity.RuntimeManager.PlayOneShot(fireWeaponData.finishReloadSound, transform.position);
    }

    /// <summary>
    /// Plays the bullet trail effect.
    /// </summary>
    /// <param name="StartPoint">The starting point of the trail.</param>
    /// <param name="EndPoint">The ending point of the trail.</param>
    /// <param name="Hit">The hit information from the raycast.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit2D Hit)
    {
        TrailRenderer instance = ResourceManager.GetBulletTrailPool().Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; // avoid position carry-over from last frame if reused

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= ResourceManager.GetBulletTrailConfig().SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        yield return new WaitForSeconds(ResourceManager.GetBulletTrailConfig().Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        ResourceManager.GetBulletTrailPool().Release(instance);
    }

    /// <summary>
    /// Handles the automatic, fire rate curve, and dispersion curve logic.
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (_wantsToFire)
        {
            if(_currentAmmo > 0){
                if (fireWeaponData.automatic)
                {
                    // Fire rate
                    if (fireWeaponData.useFireRateCurve)
                    {
                        _currentTimeToFire = fireWeaponData.fireRateCurve.Evaluate(_fireRateCurveTimer);

                        // Debug.Log(currentTimeToFire);
                        if (_fireRateTimer >= _currentTimeToFire)
                        {
                            _fireRateTimer = 0;
                            UpdateCanFire();
                        }

                        _fireRateTimer += Time.deltaTime;

                        if (_fireRateCurveTimer < fireWeaponData.fireRateCurve.GetDuration())
                            _fireRateCurveTimer += Time.deltaTime;
                    }

                    // Dispersion
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

                    // Main fire logic
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
            else
            {
                // Do semiautomatic reload
                if (!_isReloading)
                {
#if UNITY_EDITOR
                    if(log)
                        Debug.Log("Reloading");
#endif

                    // Play reload sound
                    FMODUnity.RuntimeManager.PlayOneShot(fireWeaponData.reloadSound, transform.position);

                    _isReloading = true;
                    Invoke(nameof(FinishReloading), fireWeaponData.reloadTime);
                }
            }
        }

        // Do automatic reload
        if (_currentAmmo <= 0)
        {
            if (!_isReloading)
            {
#if UNITY_EDITOR
                if (log)
                    Debug.Log("Reloading");
#endif
                _isReloading = true;
                Invoke(nameof(FinishReloading), fireWeaponData.reloadTime);
            }
        }
    }
}