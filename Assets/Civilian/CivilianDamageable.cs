using System.Collections;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Represents a civilian that can take damage and react accordingly.
/// Inherits from the Damageable class.
/// </summary>
public class CivilianDamageable : Damageable
{
    public GameObject scorepopup;

    /// <summary>
    /// Reference to the blood effect manager GameObject.
    /// </summary>
    public GameObject bloodEffectManager;

    /// <summary>
    /// Reference to the enemy weapon manager.
    /// </summary>
    private EnemyWeaponManager _enemyWeaponManager;

    /// <summary>
    /// Stores the direction of the last shot.
    /// </summary>
    private Vector3 _lastShootDir;

    /// <summary>
    /// Cooldown time for stun recovery.
    /// </summary>
    public float stunCooldown = 1.0f;

    [Header("Event")]
    [SerializeField] private UnityEvent killEvent;

    // Components to disable
    private NavMeshAgent _navMeshAgent;
    private AISensor _aiSensor;
    private BehaviourTreeRunner _behaviourTreeRunner;

    private Rigidbody2D _rb;

    /// <summary>
    /// Indicates whether the enemy is currently stunned.
    /// </summary>
    private bool _onStun = false;

    /// <summary>
    /// Initializes the EnemyDamageable instance.
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Handles the logic when the enemy is destroyed.
    /// </summary>
    public override void OnDead()
    {
        //@TODO: Change to civilian corpse

        if (SceneMng.babyMode)
        {
            Destroy(gameObject);
            return;
        }
        
        
        /*GameObject Corpse = ResourceManager.GetCivilianCorpsePool().Get();
        Corpse.transform.position = transform.position;
        Corpse.tag = "CivilianCorpse";

        Corpse.SetActive(true);

        Corpse com = Corpse.AddComponent<Corpse>();

        com.CorpseAddForceInDir(_lastShootDir);*/

        // Send kill message
        ScoreManager.AddCivilianKill();
        killEvent.Invoke();

        Instantiate(scorepopup, transform.position + new Vector3(0, 2, 10), Quaternion.identity);

        Destroy(gameObject);
    }

    /// <summary>
    /// Recovers the enemy from stun.
    /// </summary>
    private void StunRecover()
    {
        _onStun = false;

        _navMeshAgent.enabled = true;

        GetComponent<CivilianController>().enabled = true;

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
    /// <param name="dir">The direction of the stun.</param>
    public override void Stun(Vector3 dir)
    {
        if (_onStun)
            return;

        if (!_navMeshAgent)
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        _navMeshAgent.enabled = false;

        _rb = gameObject.AddComponent<Rigidbody2D>();

        _rb.drag = 3;
        _rb.gravityScale = 0;
        _rb.AddForce(dir * 200);

        _onStun = true;

        GetComponent<CivilianController>().enabled = false;

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

        // Temporal stun
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