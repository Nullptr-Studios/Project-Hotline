using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// This class acts as the manager for equipped weapons, including input
/// </summary>
public class PlayerWeaponManager : MonoBehaviour
{
    
    [Header("Pickup")]
    public float pickupRange = 2.0f;
    public LayerMask weaponLm;

    public Transform weaponHolder;

    [Header("Player")] 
    public int maxWeaponsEquipped = 2;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
#endif
    
    private bool _isWeaponHeld;
    private bool _wantsToThrowOrGet;

    private PlayerIA _playerInput;
    
    private IWeapon _heldWeaponInterface;
    private List<GameObject> _heldWeaponGameObject = new List<GameObject>();

    private int _currentIndex = 0;

    private bool _wantsToFire;
    
    private void OnDisable()
    {
        _playerInput.Gameplay.ThrowOrGet.Disable();
        _playerInput.Gameplay.Fire.Disable();
        _playerInput.Gameplay.SwitchWeapons.Disable();

    }

    // Setting all inputs and variables
    void Start()
    {
        //All maximum elements needs to be null
        for (int i = 0; i < maxWeaponsEquipped; i++)
        {
            _heldWeaponGameObject.Add(null);
        }
        
        _playerInput = new PlayerIA();
        _playerInput.Gameplay.ThrowOrGet.performed += ThrowOrGetOnPerformed;
        _playerInput.Gameplay.Fire.performed += OnFire;
        _playerInput.Gameplay.Fire.canceled += OnFire;
        _playerInput.Gameplay.SwitchWeapons.performed += SwitchWeaponsOnPerformed;
        
        _playerInput.Gameplay.ThrowOrGet.Enable();
        _playerInput.Gameplay.Fire.Enable();
        _playerInput.Gameplay.SwitchWeapons.Enable();
    }

    private void SwitchWeaponsOnPerformed(InputAction.CallbackContext context)
    {
        //Do only if performed, not cancelled
        if (context.performed)
        {
            SwitchWeapon();
        }
    }

    /// <summary>
    /// On fire logic that calls Weapon->Use(_wantsToFire)
    /// </summary>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _wantsToFire = true;
        }else if (context.canceled)
        {
            _wantsToFire = false;
        }
        
        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Use(_wantsToFire);
        }
    }
    
    private void ThrowOrGetOnPerformed(InputAction.CallbackContext context)
    {
        _wantsToThrowOrGet = context.ReadValueAsButton();
    }
    
    /// <summary>
    /// Switch weapon logic
    /// </summary>
    private void SwitchWeapon()
    {
        //In case current index is null
        if (_heldWeaponGameObject[_currentIndex] != null)
        {
            _heldWeaponGameObject[_currentIndex].gameObject.SetActive(false);
        }

        //Changes variables to default
        _heldWeaponInterface = null;
        _isWeaponHeld = false;

        //Loop around variable
        if (_currentIndex == maxWeaponsEquipped - 1)
            _currentIndex = 0;
        else
            _currentIndex++;

        //if there is something on the second slot, update variables
        if (_heldWeaponGameObject[_currentIndex] != null)
        {
            _heldWeaponGameObject[_currentIndex].gameObject.SetActive(true);
            _heldWeaponGameObject[_currentIndex].gameObject.TryGetComponent(out _heldWeaponInterface);
            _isWeaponHeld = true;
        }
    }

    // Throw and get logic
    private void Update()
    {
        if (_isWeaponHeld)
        {
            //throw
            if (_wantsToThrowOrGet)
            {
                _heldWeaponInterface.Throw(transform.right);
                _heldWeaponInterface = null;
                _heldWeaponGameObject[_currentIndex] = null;
                
                _isWeaponHeld = false;
                
                //reset input variable
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
                            if (CanEquipMoreWeapons())
                            {
                                if (!IsCurrentIndexAlreadyEquipped())
                                {
                                    _heldWeaponGameObject[_currentIndex] = hitArr[index].transform.gameObject;

                                    _heldWeaponInterface.Pickup(weaponHolder);
                                    _isWeaponHeld = true;
                                }
                                //This won't ever happen as if you have a weapon already equipped it will throw it, but just in case
                                else
                                {
                                    //Quality of life improvement
                                    SwitchWeapon();
                                    
                                    _heldWeaponGameObject[_currentIndex] = hitArr[index].transform.gameObject;

                                    _heldWeaponInterface.Pickup(weaponHolder);
                                    _isWeaponHeld = true;
                                }
                            }
                            else
                            {
                                //Debug.Log("Max Equipped");
                            }
                        }

                    }
                }
                
                //reset input variable
                _wantsToThrowOrGet = false;
            }
        }

    }

    private bool IsCurrentIndexAlreadyEquipped()
    {
        return _heldWeaponGameObject[_currentIndex] != null;
    }

    /// <summary>
    /// As _heldWeaponGameObject contains nulls we need to do this
    /// </summary>
    /// <returns></returns>
    private bool CanEquipMoreWeapons()
    {
        int notNulls = 0;
        foreach (var w in _heldWeaponGameObject)
        {
            if (w != null)
            {
                notNulls++;
            }
        }

        return notNulls < maxWeaponsEquipped;
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
                float currentDist = Vector2.Distance(hitArr[i].transform.position, weaponHolder.position);
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