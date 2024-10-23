using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyDamageable : Damageable
{
    public GameObject bloodEffectManager;

    //FUCK UNITY I DONT KNOW BUT CHILDREN DOES NOT MOVE RELATIVELY TO PARENT IF THE PARENT HAS A RIGIDBODY
    public Transform weaponHandler;

    private EnemyWeaponManager _enemyWeaponManager;

    private Vector3 _lastShootDir;

    public float stunCooldown = 1.0f;

    [SerializeField] private UnityEvent killEvent;
    
    
    //Components to disable
    private NavMeshAgent _navMeshAgent;
    private AISensor _aiSensor;
    private BehaviourTreeRunner _behaviourTreeRunner;

    private Rigidbody2D _rb;

    //Fuck unity
    private Vector3 _relativePos;
    
    private bool _onStun = false;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(weaponHandler)
            _relativePos = weaponHandler.localPosition;

        _enemyWeaponManager = gameObject.GetComponent<EnemyWeaponManager>();
    }

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

    private void StunRecover()
    {
        _onStun = false;
        
        _aiSensor.enabled = true;
        _behaviourTreeRunner.enabled = true;
        _navMeshAgent.enabled = true;
        
        Destroy(_rb);
    }

    private void Update()
    {
        //@TODO: fix fucking unity
        /*
        if (_onStun)
        {
            weaponHandler.position = gameObject.transform.position + _relativePos;
        }*/
    }

    private void Stun(Vector3 dir)
    {
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
        
        _enemyWeaponManager.DropWeapon();
        
        Invoke("StunRecover", stunCooldown);
    }

    public override void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        _lastShootDir = shootDir;
        base.DoDamage(amount);

        //temporal stun
        if (weaponType == EWeaponType.Melee && !_onStun)
        {
            Stun(shootDir);
        }

        GameObject BManager = Instantiate(bloodEffectManager, hitPoint, new Quaternion());
        BManager.transform.right = shootDir;
    }
}
