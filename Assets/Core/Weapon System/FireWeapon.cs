using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : Weapon
{

    [Header("Fire Weapon")] 
    public FireWeaponData fireWeaponData;

    public Transform gunMuzzle;
    public Transform multipleTransform;

    private int currentAmmo = 0;
    
    private float _fireRateTimer = 0.0f;
    private float _fireRateCurveTimer = 0.0f;

    private bool _wantsToFire = false;
    private bool _canFire = true;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
    [SerializeField] private bool drawGyzmos = false;
#endif
    
    public override int UsesLeft()
    {
        return currentAmmo;
    }
    
    /// <summary>
    /// Use Functionality
    /// </summary>
    public override void Use(bool pressed)
    {
        _wantsToFire = pressed;
        if (!pressed)
        {
            _fireRateTimer = 0;
            _fireRateCurveTimer = 0;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        //Implement Base class functionality
        base.Start();

        if (fireWeaponData)
        {
            currentAmmo = fireWeaponData.maxAmmo;
        }
        else
        {
            Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have fireWeaponData assigned!!!!!!!!");
        }

        if (!gunMuzzle)
        {
            Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have gunMuzzle assigned!!!!!!!!");
        }
        
    }

    private void Fire(Transform fireDir)
    {
        List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
        int ammountHits = Physics2D.Raycast(fireDir.position, fireDir.right, new ContactFilter2D(), rayHitList);

        float addedDistance = 0.0f;
        Vector2 lastHitPos = fireDir.position;

        int penetrationEnemies = 0;
        
        foreach (var hit2D in rayHitList)
        {
            int layer = hit2D.transform.gameObject.layer;
#if UNITY_EDITOR
            if (drawGyzmos)
            {
                float segmentDistance = Vector3.Distance(lastHitPos, hit2D.point);
                Debug.DrawRay(lastHitPos, fireDir.right * segmentDistance, GetRayColorDebug(hit2D.transform.gameObject.layer), 1);
                
                lastHitPos = hit2D.point;
            }
#endif

            if (layer == 6) //wall
            {
                break;
            }

            if (layer == 7 || layer == 8)   //wall-bang
            {
                continue;
            }

            if (layer == 9) //enemies
            {
                if (penetrationEnemies == fireWeaponData.penetrationAmount)
                {
                    break;
                }
                else
                {
                    //do damage
                    penetrationEnemies++;
                    continue;
                }
                    
                
            }
            
            //Default
            break;
        }
        

    }
    
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

    private void FireImplementation()
    {
        if (fireWeaponData.fireType == FireType.Simple)
        {
            Fire(gunMuzzle);
        }
        else if (fireWeaponData.fireType == FireType.Multiple)
        {
            for (int i = 0; i < fireWeaponData.fireCastsAmount; i++)
            {
                float randAngle = UnityEngine.Random.Range(-fireWeaponData.maxDispersionAngle,
                    fireWeaponData.maxDispersionAngle);

                multipleTransform.localEulerAngles = new Vector3(0, 0, randAngle);
                Fire(multipleTransform);
            }
        }
    }

    private void UpdateCanFire()
    {
        _canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_wantsToFire)
        {
            if (fireWeaponData.automatic)
            {

                if (fireWeaponData.useCurve)
                {
                    float currentTimeToFire = fireWeaponData.fireRateCurve.Evaluate(_fireRateCurveTimer);

                    Debug.Log(currentTimeToFire);
                    if (_fireRateTimer >= currentTimeToFire)
                    {
                        _fireRateTimer = 0;
                        UpdateCanFire();
                    }
                    
                    _fireRateTimer += Time.deltaTime;
                    
                    if(_fireRateCurveTimer < fireWeaponData.fireRateCurve.GetDuration())
                        _fireRateCurveTimer += Time.deltaTime;
                }
                
                if (_canFire)
                {
                    _canFire = false;
                    FireImplementation();

                    if (!fireWeaponData.useCurve)
                    {
                        Invoke("UpdateCanFire", fireWeaponData.finalFireRate);
                    }
                }
            }
            else
            {
                FireImplementation();
                _wantsToFire = false;
            }
        }
    }
}
