using System.Collections;
using FMODUnity;
using NavMeshPlus.Extensions;
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
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private static readonly int WeaponEquipped = Animator.StringToHash("WeaponEquipped");

    public GameObject bloodEffectManager;

    //FUCK UNITY I DONT KNOW BUT CHILDREN DOES NOT MOVE RELATIVELY TO PARENT IF THE PARENT HAS A RIGIDBODY
    public Transform weaponHandler;

    private EnemyWeaponManager _enemyWeaponManager;

    private Vector3 _lastShootDir;

    public float stunCooldown = 2f;

    public Animator animatorPlayer;

    public SpriteRenderer legsSpr;
    public SpriteRenderer bodySpr;

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
    
    RotateAgentSmoothly rotateSmooth2D;
    
    private AgentOverride2d override2D;

    private GameObject _player;

    [Header("Sound")] 
    public EventReference BatHitSound;

    /// <summary>
    /// Initializes the EnemyDamageable instance.
    /// </summary>
    public override void Start()
    {
        base.Start();

        _player = GameObject.FindGameObjectWithTag("Player");

        override2D = GetComponent<AgentOverride2d>();
        
        rotateSmooth2D = new RotateAgentSmoothly(override2D.Agent, override2D, 180);
        
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
        override2D.agentOverride = rotateSmooth2D;

        
        _onStun = false;

        _aiSensor.enabled = true;
        _behaviourTreeRunner.enabled = true;
        _navMeshAgent.enabled = true;
        
        animatorPlayer.SetBool(Stunned, false);
        bodySpr.sortingOrder = 1;
        
        legsSpr.enabled = true;
        
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
        override2D.agentOverride = null;
        
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
        
        _rb.freezeRotation = true;

        _rb.drag = 3;
        _rb.gravityScale = 0;
        _rb.AddForce(dir * 200);
        
        transform.up = new Vector3(transform.position.x - _player.transform.position.x, transform.position.y - _player.transform.position.y, 0).normalized;

        _onStun = true;

        GetComponent<EnemyBehaviourDataOverrider>().justStunned = true;

        _enemyWeaponManager.DropWeapon();
        
        animatorPlayer.SetBool(Stunned, true);
        animatorPlayer.SetBool(WeaponEquipped, false);
        
        legsSpr.enabled = false;
        
        bodySpr.sortingOrder = 0;

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
        //ignore damage if already stunned
        if(_onStun && weaponType == EWeaponType.Fire)
        {
            return;
        }
        
        if(weaponType == EWeaponType.Melee)
        {
            FMODUnity.RuntimeManager.PlayOneShot(BatHitSound, transform.position);
        }
        
        _lastShootDir = shootDir;
        base.DoDamage(amount);

        //temporal stun
        if (weaponType == EWeaponType.Melee)
        {
            Stun(shootDir);
        }

        GameObject BManager = ResourceManager.GetBloodManagerPool().Get();
        BManager.SetActive(true);
        BManager.transform.position = hitPoint;
        BManager.transform.right = shootDir;
    }
}