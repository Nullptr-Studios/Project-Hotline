using JetBrains.Annotations; // Used for [CanBeNull] (Unity alternative for ? variable indicator)
using System.Collections;
using CC.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
// Fucking unity it has 2 pixel perfect camera classes and the one on prod is the "experimental" -x
using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

public class PlayerHealth : Damageable
{
    public GameObject bloodEffectManager;

    public GameObject PlayerCorpsePrefab;
    
    [Header("PlayerHealth")]
    [SerializeField] private Canvas deathScreenUI;
    [SerializeField] [CanBeNull] private GameObject mainCamera;
    [SerializeField] private float deathScreenDelay = 1f;
    private bool _disableFX;
    private PlayerMovement _player;
    private PlayerIA _input;

    private Vector3 _lastShootDir;

    public bool IsDead { get; set; }

#if UNITY_EDITOR
        [Header("Debug")] [SerializeField] private bool logReload;
#endif
    
    public override void Start()
    {
        base.Start();

        _player = GetComponent<PlayerMovement>();

        if (mainCamera == null)
        {
            _disableFX = true;
            Debug.LogWarning($"[PlayerHealth] {name}: Camera not found, disabling death shader.");
        }
        
    }

    public override void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        _lastShootDir = shootDir;
        
        GameObject BManager = ResourceManager.GetBloodManagerPool().Get();
        BManager.SetActive(true);
        BManager.transform.position = hitPoint;
        BManager.transform.right = shootDir;
        
        base.DoDamage(amount, shootDir, hitPoint);
    }

    public override void OnDead()
    {
        GetComponent<PlayerWeaponManager>().DropWeapon();
        
        IsDead = true;  
        ScoreManager.AddDeath();
        //_player.OnDisable(); // Deactivates all inputs from the game
        
        GameObject c = Instantiate(PlayerCorpsePrefab, transform.position, Quaternion.identity);
        c.GetComponent<PlayerCorpse>().CorpseAddForceInDir(_lastShootDir, deathScreenUI, mainCamera?.GetComponent<PixelPerfectCamera>());
        
        gameObject.SetActive(false);
        
        //Invoke("OpenDeathScreen", deathScreenDelay);
    }

    /// <summary>
    /// Opens death screen after an amount of seconds
    /// </summary>
    private void OpenDeathScreen()
    {
        // Have to deactivate pixel perfect camera as it disables antialiasing so the shader won't work
        // TODO: Look y the shader doesnt work w/o antialiasing
        if (!_disableFX) mainCamera!.GetComponent<PixelPerfectCamera>().enabled = false;
        _input.UI.Accept.Enable();
        deathScreenUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// Reloads the scene when called
    /// </summary>
    /// <param name="context">Input context shit that doesnt work</param>
    public void RestartGame()
    {

#if UNITY_EDITOR
        if (logReload) Debug.LogWarning($"[PlayerHealth] {name}: Reloading player");
#endif

        IsDead = false;

        _currentHealth = maxHealth;
        
        deathScreenUI.gameObject.SetActive(false);
        mainCamera!.GetComponent<PixelPerfectCamera>().enabled = true;
        
        ScoreManager.Restart();
        
        GetComponent<PlayerWeaponManager>().Restart();

    }
}
