using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// This class acts as the manager for equipped weapons, including input
/// </summary>
public class WeaponManager : MonoBehaviour
{
    
    [Header("Pickup")]
    public float pickupRange = 2.0f;
    public LayerMask weaponLm;

    public Transform weaponHolder;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
#endif
    
    private bool _isWeaponHeld;
    private bool _wantsToThrowOrGet;

    private PlayerIA _playerInput;

    //@TODO: Add 2 weapon support
    private IWeapon _heldWeaponInterface;
    private GameObject _heldWeaponGameObject;




    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new PlayerIA();
        _playerInput.Gameplay.ThrowOrGet.performed += ThrowOrGetOnPerformed;
        
        _playerInput.Gameplay.ThrowOrGet.Enable();
    }

    private void ThrowOrGetOnPerformed(InputAction.CallbackContext context)
    {
        _wantsToThrowOrGet = context.ReadValue<float>() > 0.5f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isWeaponHeld)
        {
            //throw
            if (_wantsToThrowOrGet)
            {
                _heldWeaponInterface.Throw(transform.right);
                _heldWeaponInterface = null;
                _heldWeaponGameObject = null;
                
                _isWeaponHeld = false;
                _wantsToThrowOrGet = false;
            }
        }
        else
        {
            //get
            if (_wantsToThrowOrGet)
            {
                ContactFilter2D cf2D = new ContactFilter2D();
                RaycastHit2D[] hitArr = new RaycastHit2D[32];

                cf2D.SetLayerMask(weaponLm);
                cf2D.useLayerMask = true;
                
                int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(pickupRange, pickupRange),
                    CapsuleDirection2D.Horizontal,0,new Vector2(0,0),cf2D, hitArr);
                
                if(hitNumber >= 1)
                {
                    int index = DecideWeapon(hitArr, hitNumber);
                    if (index != -1)
                    {
                        if (hitArr[index].transform.TryGetComponent(out _heldWeaponInterface))
                        {
                            _heldWeaponGameObject = hitArr[index].transform.gameObject;
                            
                            _heldWeaponInterface.Pickup(weaponHolder);
                            _isWeaponHeld = true;
                        }

                    }
                }

                _wantsToThrowOrGet = false;
            }
        }

    }

    /// <summary>
    /// Checks which weapon is the closest to the player
    /// </summary>
    /// <param name="hitArr">The hit array</param>
    /// <param name="hitNumber">Amount of hits</param>
    /// <returns>The closest weapon index</returns>
    private int DecideWeapon(RaycastHit2D[] hitArr, int hitNumber)
    {
        if (hitNumber >= 2)
        {
            float smallestDistance = float.MaxValue;
            int smallestIndex = 0;
            
            //check for the closest gun
            for (int i = 0; i > hitArr.Length - 1; i++)
            {
                float currentDist = Vector2.Distance(hitArr[i].transform.position, transform.position);
                if (currentDist < smallestDistance)
                {
                    smallestDistance = currentDist;
                    smallestIndex = i;
                }
            }

            return smallestIndex;
        }
        else
        {
            return 0;
        }
    }
}
