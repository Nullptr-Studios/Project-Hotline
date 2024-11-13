using System.Collections;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Represents an enemy that can take damage and react accordingly.
/// Inherits from the Damageable class.
/// </summary>
public class EnemyDamageable : Damageable
{
    public GameObject bloodEffectManager;

    //FUCK UNITY I DONT KNOW BUT CHILDREN DOES NOT MOVE RELATIVELY TO PARENT IF THE PARENT HAS A RIGIDBODY
    public Transform weaponHandler;

    private EnemyWeaponManager _enemyWeaponManager;

    private Vector3 _lastShootDir;

    public float stunCooldown = 1.0f;

    [Header("Event")]
    [SerializeField] private UnityEvent killEvent;

    //Components to disable
    private NavMeshAgent _navMeshAgent;
    private AISensor _aiSensor;
    private BehaviourTreeRunner _behaviourTreeRunner;

    private Rigidbody2D _rb;

    //Fuck unity
    private Vector3 _relativePos;

    private bool _onStun = false;

    /// <summary>
    /// Initializes the EnemyDamageable instance.
    /// </summary>
    public override void Start()
    {
        base.Start();

        if(weaponHandler)
            _relativePos = weaponHandler.localPosition;

        _enemyWeaponManager = gameObject.GetComponent<EnemyWeaponManager>();
    }

    /// <summary>
    /// Handles the logic when the enemy is destroyed.
    /// </summary>
    public override void OnDead()
    {
        GameObject Corpse = ResourceManager.GetCorpsePool().Get();
        Corpse.transform.position = transform.position;

        Corpse.SetActive(true);

        Corpse com = Corpse.AddComponent<Corpse>();

        com.CorpseAddForceInDir(_lastShootDir);

        _enemyWeaponManager.DropWeapon();

        //Send kill message
        ScoreManager.AddKill();
        killEvent.Invoke();

        Destroy(gameObject);
    }

    /// <summary>
    /// Recovers the enemy from stun.
    /// </summary>
    private void StunRecover()
    {
        _onStun = false;

        _aiSensor.enabled = true;
        _behaviourTreeRunner.enabled = true;
        _navMeshAgent.enabled = true;

        Destroy(_rb);
    }

    /// <summary>
    /// Updates the enemy state.
    /// </summary>
    private void Update()
    {
        //@TODO: fix fucking unity
        // xd -x
        /*
        if (_onStun)
        {
            weaponHandler.position = gameObject.transform.position + _relativePos;
        }*/
    }

    /// <summary>
    /// Stuns the enemy in a specified direction.
    /// </summary>
    /// <param name="dir">The direction of the stun</param>
    public override void Stun(Vector3 dir)
    {
        if (_onStun)
            return;

        if (!_aiSensor)
            _aiSensor = gameObject.GetComponent<AISensor>();

        if (!_behaviourTreeRunner)
            _behaviourTreeRunner = gameObject.GetComponent<BehaviourTreeRunner>();

        if (!_navMeshAgent)
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        _aiSensor.enabled = false;
        _behaviourTreeRunner.enabled = false;
        _navMeshAgent.enabled = false;

        _rb = gameObject.AddComponent<Rigidbody2D>();

        _rb.drag = 3;
        _rb.gravityScale = 0;
        _rb.AddForce(dir * 200);

        _onStun = true;

        GetComponent<EnemyBehaviourDataOverrider>().justStunned = true;

        _enemyWeaponManager.DropWeapon();

        Invoke("StunRecover", stunCooldown);
    }

    /// <summary>
    /// Applies damage to the enemy and handles additional logic such as stunning and blood effects.
    /// </summary>
    /// <param name="amount">The amount of damage to apply.</param>
    /// <param name="shootDir">The direction of the shot.</param>
    /// <param name="hitPoint">The point where the shot hit.</param>
    /// <param name="weaponType">The type of weapon used.</param>
    public override void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        _lastShootDir = shootDir;
        base.DoDamage(amount);

        //temporal stun
        if (weaponType == EWeaponType.Melee && !_onStun)
        {
            Stun(shootDir);
        }

        GameObject BManager = ResourceManager.GetBloodManagerPool().Get();
        BManager.SetActive(true);
        BManager.transform.position = hitPoint;
        BManager.transform.right = shootDir;
    }
}