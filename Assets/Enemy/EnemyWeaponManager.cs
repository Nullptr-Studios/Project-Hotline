using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class acts as the manager for equipped weapons, including input.
/// </summary>
public class EnemyWeaponManager : MonoBehaviour
{
    // Animator parameters
    private static readonly int WeaponEquipped = Animator.StringToHash("WeaponEquipped");
    private static readonly int FireOrMelee = Animator.StringToHash("FireOrMelee");
    private static readonly int Type = Animator.StringToHash("Type");
    private static readonly int Use = Animator.StringToHash("Use");

    [Header("Pickup")]
    public float pickupRange = 2.0f;
    public LayerMask weaponLm;

    public Transform weaponHolder;

    public GameObject weaponGO;

    private int _maxWeaponsEquipped = 1;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
#endif

    private bool _isWeaponHeld;
    public bool _wantsToThrowOrGet;

    public Animator anim;

    private PlayerIA _playerInput;

    private IWeapon _heldWeaponInterface;
    private readonly List<GameObject> _heldWeaponGameObject = new List<GameObject>();

    private int _currentIndex;

    private bool _wantsToFire;

    private float _fireTimer;

    /// <summary>
    /// Initializes the EnemyWeaponManager, setting up weapon slots and equipping the initial weapon.
    /// </summary>
    void Start()
    {
        // All maximum elements need to be null
        for (int i = 0; i < _maxWeaponsEquipped; i++)
        {
            _heldWeaponGameObject.Add(null);
        }

        // Create already equipped weapon
        GameObject weaponToEquip = Instantiate(weaponGO);

        _heldWeaponGameObject[_currentIndex] = weaponToEquip;

        _heldWeaponInterface = weaponToEquip.GetComponent<IWeapon>();

        _heldWeaponInterface.Pickup(weaponHolder);
        _isWeaponHeld = true;
    }

    /// <summary>
    /// Drops the currently held weapon.
    /// </summary>
    public void DropWeapon()
    {
        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Drop();

            _heldWeaponInterface = null;
            _heldWeaponGameObject[_currentIndex] = null;

            _isWeaponHeld = false;
        }
    }

    /// <summary>
    /// Checks if the currently held weapon is a melee weapon.
    /// </summary>
    /// <returns>True if the held weapon is melee, false otherwise.</returns>
    public bool IsMelee()
    {
        if (!_isWeaponHeld)
            return false;

        return _heldWeaponInterface.GetWeaponType() == EWeaponType.Melee;
    }

    /// <summary>
    /// Uses the currently held weapon.
    /// </summary>
    /// <param name="fire">Indicates whether to fire the weapon.</param>
    public void useWeapon(bool fire)
    {
        // To fix the semi-automatic weapons in the enemy, we treat them as automatic weapons
        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Use(fire);
            _fireTimer = 0;
        }

        if (_isWeaponHeld && !_heldWeaponInterface.IsAutomatic())
            _heldWeaponInterface.Use(false);

        _wantsToFire = fire;

        // Animation
        // Do animation only on pressed, not on released
        if (fire)
            anim.SetTrigger(Use);
    }

    /// <summary>
    /// Updates the state of the EnemyWeaponManager, handling weapon usage and switching.
    /// </summary>
    private void Update()
    {
        // Animation
        anim.SetBool(WeaponEquipped, _isWeaponHeld);
        if (_isWeaponHeld)
        {
            anim.SetBool(FireOrMelee, _heldWeaponInterface.GetWeaponType() == EWeaponType.Fire);
            anim.SetInteger(Type, _heldWeaponInterface.GetWeaponSpriteID());
        }

        if (_isWeaponHeld && _wantsToFire && !_heldWeaponInterface.IsAutomatic())
        {
            if (_fireTimer >= _heldWeaponInterface.TimeBetweenUses())
            {
                _heldWeaponInterface.Use(true);
                _fireTimer = 0;
            }
            else
            {
                _fireTimer += Time.deltaTime;
            }
        }

        if (_isWeaponHeld)
        {
            // Throw
            if (!_wantsToThrowOrGet) return;

            _heldWeaponInterface.Throw(transform.right);
            _heldWeaponInterface = null;
            _heldWeaponGameObject[_currentIndex] = null;

            _isWeaponHeld = false;

            // Reset input variable
            _wantsToThrowOrGet = false;
        }
        else
        {
            // Get
            if (!_wantsToThrowOrGet) return;

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

                                _heldWeaponInterface.setClaimed(true);

                                _isWeaponHeld = true;
                            }
                        }
                    }
                }
            }

            // Reset input variable
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

        return notNulls < _maxWeaponsEquipped;
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

            int i = 0;

            // Check for the closest gun from the array
            foreach (var h in hitArr)
            {
                // Null check
                if (!h)
                    continue;

                float currentDist = Vector2.Distance(h.transform.position, weaponHolder.position);
                if (currentDist < smallestDistance)
                {
                    smallestDistance = currentDist;
                    smallestIndex = i;
                    i++;
                }
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
            Gizmos.DrawWireSphere(transform.position, pickupRange / 2);
    }
#endif
}