using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class acts as the manager for equipped weapons, including input.
/// </summary>
public class PlayerWeaponManager : MonoBehaviour
{
    // Animator parameters
    private static readonly int WeaponEquipped = Animator.StringToHash("WeaponEquipped");
    private static readonly int FireOrMelee = Animator.StringToHash("FireOrMelee");
    private static readonly int Type = Animator.StringToHash("Type");
    private static readonly int Use = Animator.StringToHash("Use");

    public GameObject spawningWeapon;
    public WeaponDictionary weaponDictionary;

    [Header("Pickup")]
    public float pickupRange = 2.0f;
    public LayerMask weaponLm;

    public Transform weaponHolder;

    [Header("Player")]
    public int maxWeaponsEquipped = 2;

    [Header("Sound")]
    public EventReference pickupSound;
    public EventReference throwSound;
    public EventReference switchSound;

    [Header("Animation")]
    public Animator anim;

    [Header("Cursor")]
    public Texture2D MeleeCursor;
    public Texture2D FireCursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;


#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
#endif

    private bool _isWeaponHeld;
    private bool _wantsToThrowOrGet;
    /*[SerializeField] ProgressBar progressBar;
    [SerializeField] ProgressBar progressBar2;*/
    [SerializeField] AmmoPrompt ammoPrompt;

    private PlayerIA _playerInput;

    private IWeapon _heldWeaponInterface;
    public readonly List<GameObject> _heldWeaponGameObject = new List<GameObject>();

    private int _currentIndex;

    private bool _wantsToFire;

    private bool _reloading;

    private float _bulletsUITimer = 0.0f;

    private int _lastReloadingWeaponIndex;
    private GameObject _lastReloadingWeaponGO;

    private float _timeToShootAgain;
    private float _timerAgain;
    private bool _canShootAgain = true;
    
    public EWeaponType GetCurrentWeaponType()
    {
        if(_heldWeaponInterface == null)
            return EWeaponType.Default;
        
        return _heldWeaponInterface.GetWeaponType();
    }

    public void DropWeapon()
    {
        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Drop();
            ammoPrompt.SetSlotEmpty(_currentIndex);
            OnChange();
        }
    }

    /// <summary>
    /// Enables the player input.
    /// </summary>
    public void EnableInput()
    {
        _playerInput.Gameplay.ThrowOrGet.Enable();
        _playerInput.Gameplay.Fire.Enable();
        _playerInput.Gameplay.SwitchWeapons.Enable();
    }

    /// <summary>
    /// Disables the player input.
    /// </summary>
    public void DisableInput()
    {
        _playerInput.Gameplay.ThrowOrGet.Disable();
        _playerInput.Gameplay.Fire.Disable();
        _playerInput.Gameplay.SwitchWeapons.Disable();

    }

    public void OnChange()
    {
        if (_heldWeaponInterface == null)
        {
            Cursor.SetCursor(FireCursor, hotSpot, cursorMode);
            return;
        }

        if (_heldWeaponInterface.GetWeaponType() == EWeaponType.Melee)
        {
            Cursor.SetCursor(MeleeCursor, hotSpot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(FireCursor, hotSpot, cursorMode);

        }
    }

    public void SpawnWeapons(List<string> weaponsNames)
    {
        for (int i = 0; i < _heldWeaponGameObject.Count; i++)
        {
            if (_heldWeaponGameObject[i] != null)
            {
                _heldWeaponGameObject[i].SetActive(true);
                if(_heldWeaponGameObject[i].TryGetComponent(out IWeapon w))
                    w.Reload();
                _heldWeaponGameObject[i] = null;
            }
        }
        
        _isWeaponHeld = false;
        _heldWeaponInterface = null;
        
        for (int i = 0; i < weaponsNames.Count; i++)
        {
            if(weaponsNames[i] == null)
                continue;
            
            ammoPrompt.SetSlotEmpty(0);
            ammoPrompt.SetSlotEmpty(1);
            
            foreach (var w in weaponDictionary.weapons)
            {
                if (weaponsNames[i].Contains(w.name))
                {
                    GameObject weaponToEquip = Instantiate(w);
                    _heldWeaponGameObject[i] = weaponToEquip;
                    _heldWeaponInterface = weaponToEquip.GetComponent<IWeapon>();
                    _heldWeaponInterface.Pickup(weaponHolder);
                    _heldWeaponInterface.SetIsPlayer(true);
                    

                    //FMODUnity.RuntimeManager.PlayOneShot(pickupSound, transform.position);

                    _heldWeaponInterface.setClaimed(true);
                    break;
                }
            }
        }
        
        _currentIndex = 0;
        _heldWeaponInterface = _heldWeaponGameObject[_currentIndex].GetComponent<IWeapon>();
        _isWeaponHeld = true;
        
        ammoPrompt.ChangeActiveSlot(_currentIndex);
        ammoPrompt.SetSlot(_currentIndex, _heldWeaponGameObject[_currentIndex].name.Split("(".ToCharArray())[0]);
        
        anim.ResetTrigger(Use);
    }

    /// <summary>
    /// Initializes the player weapon manager.
    /// </summary>
    void Awake()
    {
        // Initialize weapon slots
        for (int i = 0; i < maxWeaponsEquipped; i++)
        {
            _heldWeaponGameObject.Add(null);
        }

        // Placeholder for spawning weapon
        if (spawningWeapon)
        {
            /*Instantiate(spawningWeapon, transform.position, new Quaternion());
            _wantsToThrowOrGet = true;*/
            
            SpawnWeapons( new List<string> { spawningWeapon.name });
        }
        //###############################################
        OnChange();
    }


    public void Restart()
    {
        SpawnWeapons(SceneMng.CheckpointWeapons);
        SetAmmoPrompt();
    }


    public void SetAmmoPrompt()
    {
        if(!_isWeaponHeld)
            return;
        if (_heldWeaponInterface.MaxUses() != -1)
        {
            ammoPrompt.SetMaxAmmo(_heldWeaponInterface.MaxUses(), 8);
            ammoPrompt.SetCurrentAmmo(_heldWeaponInterface.UsesLeft());
        }
        else
            ammoPrompt.DoHide();
        
        ammoPrompt.SetSlot(_currentIndex, _heldWeaponGameObject[_currentIndex].name.Split("(".ToCharArray())[0]);
        OnChange();
    }

    private void Start()
    {
        SetAmmoPrompt();
    }

    /// <summary>
    /// Handles the switch weapons input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    private void SwitchWeaponsOnPerformed(InputAction.CallbackContext context)
    {
        //Do only if performed, not cancelled
        if (context.performed)
        {
            SwitchWeapon();
            OnChange();
        }
    }

    /// <summary>
    /// Handles the fire input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && _isWeaponHeld)
        {
            if(_heldWeaponInterface.GetWeaponType() == EWeaponType.Melee && !_canShootAgain)
                _wantsToFire = false;
            else
                _wantsToFire = true;
            
            
            _bulletsUITimer = 0;
            if (_isWeaponHeld)
            {
                if (_heldWeaponInterface.UsesLeft() == 0 && _heldWeaponInterface.GetWeaponType() == EWeaponType.Fire)
                {
                    FireWeapon fireWeapon = (FireWeapon) _heldWeaponInterface;
                    FMODUnity.RuntimeManager.PlayOneShot(fireWeapon.fireWeaponData.emptyClipSound, transform.position);
                }
            }
        }
        else if (context.canceled)
        {
            _wantsToFire = false;
        }

        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Use(_wantsToFire);
            if (!_heldWeaponInterface.IsAutomatic())
            {
                _wantsToFire = false;

                _timeToShootAgain = _heldWeaponInterface.TimeBetweenUses();

                _canShootAgain = false;
            }
        }

        if (context.performed)
            anim.SetTrigger(Use);
        
        if(_isWeaponHeld) 
            ammoPrompt.SetCurrentAmmo(_heldWeaponInterface.UsesLeft());
    }

    /// <summary>
    /// Handles the throw or get input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    private void ThrowOrGetOnPerformed(InputAction.CallbackContext context)
    {
        _wantsToThrowOrGet = context.ReadValueAsButton();
        OnChange();
    }

    /// <summary>
    /// Switches the currently held weapon.
    /// </summary>
    private void SwitchWeapon()
    {
        //In case current index is null
        if (_heldWeaponGameObject[_currentIndex] != null)
        {
            _heldWeaponInterface.Use(false);
            _heldWeaponGameObject[_currentIndex].gameObject.SetActive(false);
        }

        //Changes variables to default
        _heldWeaponInterface = null;
        _isWeaponHeld = false;

        //temporal fix :)
        _canShootAgain = true;

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

            //FMODUnity.RuntimeManager.PlayOneShot(switchSound, transform.position);

            if (_heldWeaponInterface.MaxUses() != -1 && _heldWeaponInterface.UsesLeft() != 0)
            {
                ammoPrompt.SetMaxAmmo(_heldWeaponInterface.MaxUses(), 8);
         
                
                ammoPrompt.SetCurrentAmmo(_heldWeaponInterface.UsesLeft());
                
            }
            else
                ammoPrompt.DoHide();
        }
        else
        {
            ammoPrompt.DoHide();
        }

        ammoPrompt.ChangeActiveSlot(_currentIndex);
        anim.ResetTrigger(Use);
    }

    /// <summary>
    /// Initializes the player input actions and binds the input events to their respective handlers.
    /// </summary>
    public void InitializeInput()
    {
        _playerInput = new PlayerIA();
        _playerInput.Gameplay.ThrowOrGet.performed += ThrowOrGetOnPerformed;
        _playerInput.Gameplay.Fire.performed += OnFire;
        _playerInput.Gameplay.Fire.canceled += OnFire;
        _playerInput.Gameplay.SwitchWeapons.performed += SwitchWeaponsOnPerformed;
    }

    // Throw and get logic
    // I dont like this implementation on the fucking update cuz i cant call it  -x
    /// <summary>
    /// WHYYYYYYYYYYYY IS THE FUCKING UPDATE METHOD NOT BEING CALLED -D
    /// </summary>
    private void Update()
    {
        
        anim.SetBool(WeaponEquipped, _isWeaponHeld);
        if (_isWeaponHeld)
        {
            anim.SetBool(FireOrMelee, _heldWeaponInterface.GetWeaponType() == EWeaponType.Fire);
            if (_heldWeaponInterface.GetWeaponSpriteID() >= 0)
            {
                anim.SetInteger(Type, _heldWeaponInterface.GetWeaponSpriteID());
            }
            else
            {
                anim.SetBool(WeaponEquipped, false);
            }
        }

        if (_isWeaponHeld)
        {
            if (_wantsToFire && _heldWeaponInterface.IsAutomatic())
            {
                if (_bulletsUITimer >= _heldWeaponInterface.TimeBetweenUses())
                {
                    _bulletsUITimer = 0;
                    ammoPrompt.SubtractBullet();
                }
                else
                {
                    _bulletsUITimer += Time.deltaTime;
                }
            }
        }

        if (_isWeaponHeld && !_heldWeaponInterface.IsAutomatic())
        {
            if (!_canShootAgain)
            {
                if (_timerAgain >= _timeToShootAgain)
                {
                    _canShootAgain = true;
                    _timerAgain = 0;
                }
                else
                {
                    _timerAgain += Time.deltaTime;
                }
            }
        }

        if (_isWeaponHeld)
        {
            if (!_wantsToThrowOrGet)
                return;

            _heldWeaponInterface.Throw(transform.right);
            _heldWeaponInterface = null;
            _heldWeaponGameObject[_currentIndex] = null;
            ammoPrompt.SetSlotEmpty(_currentIndex);

            FMODUnity.RuntimeManager.PlayOneShot(throwSound, transform.position);

            _isWeaponHeld = false;
            
            //temporal fix :)
            _canShootAgain = true;

            ammoPrompt.DoHide();

            anim.ResetTrigger(Use);

            OnChange();

            _wantsToThrowOrGet = false;
        }
        else
        {
            if (!_wantsToThrowOrGet)
                return;

            ContactFilter2D cf2D = new ContactFilter2D();
            RaycastHit2D[] hitArr = new RaycastHit2D[32];

            cf2D.SetLayerMask(weaponLm);
            cf2D.useLayerMask = true;

            int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(pickupRange, pickupRange),
                CapsuleDirection2D.Horizontal, 0, new Vector2(0, 0), cf2D, hitArr);

            if (hitNumber >= 1)
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
                                
                                _heldWeaponInterface.SetIsPlayer(true);

                                if (_heldWeaponInterface.MaxUses() != -1)
                                {
                                    ammoPrompt.SetMaxAmmo(_heldWeaponInterface.MaxUses(), 8);
                                    ammoPrompt.SetCurrentAmmo(_heldWeaponInterface.UsesLeft());
                                    
                                }
                                else
                                    ammoPrompt.DoHide();

                                _isWeaponHeld = true;

                                FMODUnity.RuntimeManager.PlayOneShot(pickupSound, transform.position);

                                OnChange();

                                _heldWeaponInterface.setClaimed(true);
                                
                                ammoPrompt.SetSlot(_currentIndex, _heldWeaponGameObject[_currentIndex].name.Split("(".ToCharArray())[0]);
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
                        // else
                        // {
                        //     //Debug.Log("Max Equipped");
                        // }
                    }

                }
            }

            _wantsToThrowOrGet = false;
        }
    }

    /// <summary>
    /// Checks if the current index already has a weapon equipped.
    /// </summary>
    /// <returns>True if the current index already has a weapon equipped, false otherwise.</returns>
    private bool IsCurrentIndexAlreadyEquipped()
    {
        return _heldWeaponGameObject[_currentIndex] != null;
    }

    /// <summary>
    /// Checks if more weapons can be equipped.
    /// </summary>
    /// <returns>True if more weapons can be equipped, false otherwise.</returns>
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
    /// Decides which weapon is the closest to the player.
    /// </summary>
    /// <param name="hitArr">The hit array.</param>
    /// <param name="hitNumber">The number of hits.</param>
    /// <returns>The index of the closest weapon.</returns>
    private int DecideWeapon(RaycastHit2D[] hitArr, int hitNumber)
    {
        if (hitNumber >= 2)
        {
            float smallestDistance = float.MaxValue;
            int smallestIndex = 0;

            int mostAmmo = -69;
            
            int i = 0;

            foreach (var h in hitArr)
            {
                if (!h)
                    continue;
                
                //smallest distance
                /*float currentDist = Vector2.Distance(h.transform.position, weaponHolder.position);
                if (currentDist < smallestDistance)
                {
                    smallestDistance = currentDist;
                    smallestIndex = i;
                }*/
                
                h.transform.gameObject.TryGetComponent(out IWeapon w);
                if(w.UsesLeft() > mostAmmo)
                {
                    mostAmmo = w.UsesLeft();
                    smallestIndex = i;
                }
                
                i++;
            }

            return smallestIndex;
        }
        else
        {
            return 0;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws gizmos for debugging purposes.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, pickupRange / 2);
            
            Gizmos.color = Color.red;
            if(_heldWeaponInterface != null)
                Gizmos.DrawWireSphere(transform.position, _heldWeaponInterface.GetHearingRange() / 2);
        }
    }
#endif
}