using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyWeaponManager : MonoBehaviour
{
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

    public bool _isWeaponHeld;
    public bool _wantsToThrowOrGet;
    public Animator anim;

    private PlayerIA _playerInput;
    private IWeapon _heldWeaponInterface;
    private readonly List<GameObject> _heldWeaponGameObject = new List<GameObject>();
    private int _currentIndex;
    private bool _wantsToFire;
    private float _fireTimer;

    public delegate void GotWeapon();
    public GotWeapon GotWeaponDelegate;

    void Start()
    {
        for (int i = 0; i < _maxWeaponsEquipped; i++)
        {
            _heldWeaponGameObject.Add(null);
        }

        GameObject weaponToEquip = Instantiate(weaponGO);
        _heldWeaponGameObject[_currentIndex] = weaponToEquip;
        _heldWeaponInterface = weaponToEquip.GetComponent<IWeapon>();
        _heldWeaponInterface.Pickup(weaponHolder);
        _isWeaponHeld = true;
    }

    public void OnlyGetWeapon()
    {
        ContactFilter2D cf2D = new ContactFilter2D();
        RaycastHit2D[] hitArr = new RaycastHit2D[32];
        cf2D.SetLayerMask(weaponLm);
        cf2D.useLayerMask = true;

        int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(pickupRange, pickupRange),
            CapsuleDirection2D.Horizontal, 0, Vector2.zero, cf2D, hitArr);

        if (hitNumber >= 1)
        {
            int index = DecideWeapon(hitArr, hitNumber);
            if (index != -1 && hitArr[index].transform.TryGetComponent(out _heldWeaponInterface))
            {
                if (CanEquipMoreWeapons() && !IsCurrentIndexAlreadyEquipped())
                {
                    _heldWeaponGameObject[_currentIndex] = hitArr[index].transform.gameObject;
                    _heldWeaponInterface.Pickup(weaponHolder);
                    _heldWeaponInterface.SetIsPlayer(false);
                    _heldWeaponInterface.setClaimed(true);
                    _isWeaponHeld = true;
                }
            }
        }

        GotWeaponDelegate?.Invoke();
        _wantsToThrowOrGet = false;
    }

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

    public bool IsMelee()
    {
        return _isWeaponHeld && _heldWeaponInterface.GetWeaponType() == EWeaponType.Melee;
    }

    public void useWeapon(bool fire)
    {
        if (_isWeaponHeld)
        {
            _heldWeaponInterface.Use(fire);
            _fireTimer = 0;
        }

        if (_isWeaponHeld && !_heldWeaponInterface.IsAutomatic())
            _heldWeaponInterface.Use(false);

        _wantsToFire = fire;

        if (fire)
            anim.SetTrigger(Use);
    }

    private void Update()
    {
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
            if (!_wantsToThrowOrGet) return;

            _heldWeaponInterface.Throw(transform.right);
            _heldWeaponInterface = null;
            _heldWeaponGameObject[_currentIndex] = null;
            _isWeaponHeld = false;
            _wantsToThrowOrGet = false;
        }
        else
        {
            if (!_wantsToThrowOrGet) return;

            ContactFilter2D cf2D = new ContactFilter2D();
            RaycastHit2D[] hitArr = new RaycastHit2D[32];
            cf2D.SetLayerMask(weaponLm);
            cf2D.useLayerMask = true;

            int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(pickupRange, pickupRange),
                CapsuleDirection2D.Horizontal, 0, Vector2.zero, cf2D, hitArr);

            if (hitNumber >= 1)
            {
                int index = DecideWeapon(hitArr, hitNumber);
                if (index != -1 && hitArr[index].transform.TryGetComponent(out _heldWeaponInterface))
                {
                    if (CanEquipMoreWeapons() && !IsCurrentIndexAlreadyEquipped())
                    {
                        _heldWeaponGameObject[_currentIndex] = hitArr[index].transform.gameObject;
                        _heldWeaponInterface.Pickup(weaponHolder);
                        _heldWeaponInterface.SetIsPlayer(false);
                        _heldWeaponInterface.setClaimed(true);
                        _isWeaponHeld = true;
                    }
                }
            }

            GotWeaponDelegate?.Invoke();
            _wantsToThrowOrGet = false;
        }
    }

    private bool IsCurrentIndexAlreadyEquipped()
    {
        return _heldWeaponGameObject[_currentIndex] != null;
    }

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

    private int DecideWeapon(RaycastHit2D[] hitArr, int hitNumber)
    {
        if (hitNumber >= 2)
        {
            float smallestDistance = float.MaxValue;
            int smallestIndex = 0;

            for (int i = 0; i < hitNumber; i++)
            {
                if (!hitArr[i]) continue;

                float currentDist = Vector2.Distance(hitArr[i].transform.position, weaponHolder.position);
                if (currentDist < smallestDistance)
                {
                    smallestDistance = currentDist;
                    smallestIndex = i;
                }
            }

            return smallestIndex;
        }
        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawGizmos)
            Gizmos.DrawWireSphere(transform.position, pickupRange / 2);
    }
#endif
}