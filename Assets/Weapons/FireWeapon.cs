using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireWeapon : Weapon
{

    [Header("Fire Weapon")] 
    public FireWeaponData fireWeaponData;

    //Gun muzzle wil be used for FX
    public Transform gunMuzzle;
    //The transform where the ray casts will be emited
    public Transform dispersionTransform;

    private int _currentAmmo = 0;
    private bool _isReloading = false;

    //Fire rate global variables
    private float _fireRateTimer = 0.0f;
    private float _fireRateCurveTimer = 0.0f;

    //@TODO: Fix initial spamming
    private bool _wantsToFire = false;
    private bool _canFire = true;

    //Dispersion global variables
    private float _currentDispersion = 0.0f;
    private float _dispersionCurveTimer = 0.0f;

    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
    [SerializeField] private bool drawGyzmos = false;
    [SerializeField] private float gyzmosDuration = .5f;
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
        int ammountHits = Physics2D.Raycast(fireDir.position, fireDir.right, new ContactFilter2D(), rayHitList);
        
        //Initial Position
        Vector2 lastHitPos = fireDir.position;

        //Amount of penetrated enemies
        int penetrationEnemies = 0;
        
        foreach (var hit2D in rayHitList)
        {
            int layer = hit2D.transform.gameObject.layer;
            
            //Gyzmos shit
#if UNITY_EDITOR
            if (drawGyzmos)
            {
                float segmentDistance = Vector3.Distance(lastHitPos, hit2D.point);
                Debug.DrawRay(lastHitPos, fireDir.right * segmentDistance, GetRayColorDebug(hit2D.transform.gameObject.layer), gyzmosDuration);
                
                lastHitPos = hit2D.point;
            }
#endif
            IDamageable damageableInterface;
            hit2D.transform.TryGetComponent(out damageableInterface);

            if (damageableInterface != null)
            {
                damageableInterface.DoDamage(1);
            }

            if (layer == 6) //wall
            {  
                //If it's a wall we want to return, no more looping
                break;
            }

            if (layer == 7 || layer == 8)   //wall-bang
            {
                //if bullets can penetrate continue loop
                continue;
            }

            if (layer == 9) //enemies
            {
                //Calculate penetration
                if (penetrationEnemies == fireWeaponData.penetrationAmount)
                {
                    break;
                }
                else
                {
                    penetrationEnemies++;
                    continue;
                }
                    
                
            }
            
            //We treat default objects as walls
            break;
        }
        

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
            float randAngle = UnityEngine.Random.Range(-_currentDispersion,
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
                            Invoke("UpdateCanFire", fireWeaponData.finalFireRate);
                        }
                    }
                }
                else
                {
                    if (_canFire)
                    {
                        _currentDispersion = fireWeaponData.maxDispersionAngle;

                        Invoke("UpdateCanFire", fireWeaponData.finalFireRate);
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
                    Invoke("FinishReloading", fireWeaponData.reloadTime);
                }
            }
        }
    }
}
