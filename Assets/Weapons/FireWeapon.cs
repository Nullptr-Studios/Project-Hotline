using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class FireWeapon : Weapon
{

    [Header("Fire Weapon")] 
    public FireWeaponData fireWeaponData;

    //Gun muzzle wil be used for FX
    public Transform gunMuzzle;
    //The transform where the ray casts will be emitted
    public Transform dispersionTransform;

    private int _currentAmmo;
    private bool _isReloading;

    //Fire rate global variables
    private float _fireRateTimer;
    private float _fireRateCurveTimer;

    //@TODO: Fix initial spamming
    private bool _wantsToFire;
    private bool _canFire = true;

    //Dispersion global variables
    private float _currentDispersion;
    private float _dispersionCurveTimer;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
    [SerializeField] private float gizmosDuration = .5f;
#endif
    
    public override int UsesLeft()
    {
        return _currentAmmo;
    }

    /// <summary>
    /// Use Functionality
    /// </summary>
    public override void Use(bool pressed)
    {
        _wantsToFire = pressed;
        if (pressed)
        {
            if (fireWeaponData.useFireRateCurve)
            {
                //@TODO: Fix Spamming issue
                //so it fires as soon as it´s pressed
                _fireRateTimer = fireWeaponData.fireRateCurve.Evaluate(0);
            }
        }
        else
        {
            _fireRateTimer = 0;
            _fireRateCurveTimer = 0;

            _dispersionCurveTimer = 0;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        //Implement Base class functionality
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
        
#if UNITY_EDITOR
        if (!gunMuzzle)
        {
            if(log)
                Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have gunMuzzle assigned!!!!!!!!");
        }
#endif        
        
    }

    private void Fire(Transform fireDir)
    {
        //Raycast2D list
        List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
        int amountHits = Physics2D.Raycast(fireDir.position, fireDir.right, new ContactFilter2D(), rayHitList);

        if (amountHits == 0)
        {
            //Do trail even if we aren't hitting anything
            StartCoroutine(PlayTrail(fireDir.position, fireDir.position + fireDir.right * ResourceManager.GetBulletTrailConfig().MissDistance, new RaycastHit2D()));
            return;
        }
        
        //Initial Position
        Vector2 lastHitPos = fireDir.position;

        //Amount of penetrated enemies
        int currentPenetration = 0;
        
        foreach (var hit2D in rayHitList)
        {
            int layer = hit2D.transform.gameObject.layer;
            
            //Gizmos shit
#if UNITY_EDITOR
            if (drawGizmos)
            {
                float segmentDistance = Vector3.Distance(lastHitPos, hit2D.point);
                Debug.DrawRay(lastHitPos, fireDir.right * segmentDistance, GetRayColorDebug(hit2D.transform.gameObject.layer), gizmosDuration);
                
                lastHitPos = hit2D.point;
            }
#endif
            //Do damage to all surfaces
            if (hit2D.transform.TryGetComponent(out IDamageable damageableInterface))
                damageableInterface.DoDamage(1, fireDir.right, hit2D.point);

            if (layer == 6) //wall
            {  
                //If it's a wall we want to return, no more looping
                
                //Do trail
                StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                break;
            }

            if (layer == 7 || layer == 8)   //wall-bang
            {
                //if bullets can penetrate continue loop

                //Calculate penetration
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    //Do trail
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                currentPenetration++;
                continue;
            }

            if (layer == 9) //enemies
            {

                //Calculate penetration
                if (currentPenetration == fireWeaponData.penetrationAmount)
                {
                    //Do trail
                    StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
                    break;
                }

                currentPenetration++;
                continue;


            }
            
            //Do trail
            StartCoroutine(PlayTrail(fireDir.position, hit2D.point, hit2D));
            
            //We treat default objects as walls
            break;
        }

        /* There is a bug that does not render trails if there is no more colliders, wont fix since the game will take place indoors */
        

    }
    
#if UNITY_EDITOR
    // Function to determine ray color based on layer
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
    /// Calculate random dispersion
    /// </summary>
    private void RandomDispersion()
    {
        if (fireWeaponData.useDispersion)
        {
            float randAngle = Random.Range(-_currentDispersion,
                _currentDispersion);

            dispersionTransform.localEulerAngles = new Vector3(0, 0, randAngle);
        }
        else
        {
            dispersionTransform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// Logic between simple and multiple types of fires
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
        
        //subtract current ammo
        _currentAmmo--;

    }

    /// <summary>
    /// Called via Invoke
    /// </summary>
    private void UpdateCanFire()
    {
        _canFire = true;
    }

    /// <summary>
    /// Finished reloading logic, called via Invoke
    /// </summary>
    private void FinishReloading()
    {
#if UNITY_EDITOR
        if(log)
            Debug.Log("Reloaded!!");
#endif
        _isReloading = false;
        _currentAmmo = fireWeaponData.maxAmmo;
        
        //Reset variables to 0
        _fireRateTimer = 0;
        _fireRateCurveTimer = 0;

        _dispersionCurveTimer = 0;
    }
    
    //Trail
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

        if (Hit.collider != null)
        {
            //@TODO: Do impact
        }

        yield return new WaitForSeconds(ResourceManager.GetBulletTrailConfig().Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        ResourceManager.GetBulletTrailPool().Release(instance);
    }


    //@TODO: Fix initial spamming
    /// <summary>
    /// Automatic, fire rate curve and dispersion curve logic
    /// </summary>
    void Update()
    {
        if (_wantsToFire)
        {
            if(_currentAmmo > 0){
                if (fireWeaponData.automatic)
                {

                    //Fire rate
                    if (fireWeaponData.useFireRateCurve)
                    {
                        float currentTimeToFire = fireWeaponData.fireRateCurve.Evaluate(_fireRateCurveTimer);

                        //Debug.Log(currentTimeToFire);
                        if (_fireRateTimer >= currentTimeToFire)
                        {
                            _fireRateTimer = 0;
                            UpdateCanFire();
                        }

                        _fireRateTimer += Time.deltaTime;

                        if (_fireRateCurveTimer < fireWeaponData.fireRateCurve.GetDuration())
                            _fireRateCurveTimer += Time.deltaTime;
                    }

                    //Dispersion
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

                    //Main fire logic
                    if (_canFire)
                    {
                        _canFire = false;
                        FireImplementation();

                        if (!fireWeaponData.useFireRateCurve)
                        {
                            Invoke(nameof(UpdateCanFire), fireWeaponData.finalFireRate);
                        }
                    }
                }
                else
                {
                    if (_canFire)
                    {
                        _currentDispersion = fireWeaponData.maxDispersionAngle;

                        Invoke(nameof(UpdateCanFire), fireWeaponData.finalFireRate);
                        FireImplementation();
                        
                        _canFire = false;
                        _wantsToFire = false;
                    }
                }
            }
            else
            {
                //Do automatic reload
                if (!_isReloading)
                {
#if UNITY_EDITOR
                    if(log)
                        Debug.Log("Reloading");
#endif
                    _isReloading = true;
                    Invoke(nameof(FinishReloading), fireWeaponData.reloadTime);
                }
            }
        }
    }
}
