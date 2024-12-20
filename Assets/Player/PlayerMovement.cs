using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    
    private PlayerIA _input;
    [FormerlySerializedAs("_inputComponent")] [SerializeField] private PlayerInput inputComponent;
    private Rigidbody2D _rb;
    private Camera _camera;
    private PlayerWeaponManager _weaponManager;

    /// <summary>
    /// Returns controller being used
    /// </summary>
    public static EController Controller { get; private set; }

    [Header("Player Movement")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private MotionController movementController;
    [SerializeField] private float inputSmoothTime = 0.2f;
    private Vector2 _inputDir;
    private Vector2 _directionTarget;
    private Vector2 _directionSmooth;
    private Vector2 _velocity;

    [Header("Animation")] 
    public Animator playerAnimation;
    
    public Transform footTransform;
    public Animator footAnimation;
    
    private static readonly int AnimRate = Animator.StringToHash("AnimRate");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    [Header("Player Aim")]
    [SerializeField] private float aimSmoothTime = 0.2f;
    [SerializeField] private float aimMinimumDistance = 0.2f;
    [Tooltip("This makes the player rotate from its center rather than from its collider")]
    [SerializeField] private bool pivotRotation;
    [SerializeField] private GameObject sprite;
    [SerializeField] private Collider2D playerCollider;
    private Vector2 _rawControllerPosition;
    private Vector2 _rawMousePosition;
    private Vector3 _currentDir;
    private Vector3 _dir;
    
    private GameObject _pauseMenu;
    
    private float _footstepTimer;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugSpeed;
    [SerializeField] private bool debugInput;
#endif

    private void Awake()
    {
        _input = new PlayerIA();
        _rb = GetComponent<Rigidbody2D>();
        _camera = GameObject.Find("Cinemachine Brain").GetComponent<Camera>();
        _weaponManager = GetComponent<PlayerWeaponManager>();
        // Decided to do this here because it was causing issues on the weapon manager -x
        // Unity decided to do this OnEnable before the Awake on the weapon manager so it crashed -x
        // Don't know why it does that now, but it does -x
        _weaponManager.InitializeInput();
        
        _input.Debug.Debug.performed += OnDebug;
        _input.Debug.Restart.performed += ForceRestart;
        _input.Gameplay.Movement.performed += OnMove;
        _input.Gameplay.Movement.canceled += OnMove;
        _input.Gameplay.Aim.performed += OnAim;
        _input.Gameplay.AimMouse.performed += OnAimMouse;
        _input.Gameplay.Pause.performed += OnOpenPause;
        _input.Gameplay.ReturnToMainMenu.performed += OnRetMain;
        // _input.Gameplay.Aim.canceled += OnAim;

        // The fact that i have to use this class for a fucking controller change makes me mad -x
        // like VERY VERY mad in a level that if i were to write it on this comment i would get banned from digipen -x
        inputComponent.onControlsChanged += OnChangeController;

        // This gives us the ability to choose the pivot from center to body center
        // We'll see which one feels better
        if (pivotRotation)
        {
            playerCollider.offset = new Vector2(-0.2f, 0f);
            sprite.transform.localPosition = Vector3.zero;
        }

        switch (inputComponent.currentControlScheme)
        {
            case "KeyboardMouse":
                Controller = EController.KeyboardMouse;
                break;
            case "Dualsense":
                Controller = EController.Dualsense;
                break;
            default:
                Controller = EController.Xbox;
                return;
        }
        
        // Pause menu shit
        // Have to do this since i cant rescale the canvas -x
        _pauseMenu = GameObject.Find("PA_PauseMenu").GetComponentInChildren<PauseMenu>().gameObject;
        _pauseMenu.SetActive(false);
    }

    // TODO: THIS SHOULD ABSOFUCKINGLUTLY NOT BE ON PROD 
    private void OnRetMain(InputAction.CallbackContext obj)
    {
        GameObject.Find("PA_LevelManager").SendMessage("EndLevelMessage");
    }

    /// <summary>
    /// Handles controller change and stores it on an enum
    /// </summary>
    /// <param name="o">Idk what is this but it works</param>
    private void OnChangeController(PlayerInput o)
    {
        switch (o.currentControlScheme)
        {   // Cases needs to be named exactly the same as on InputAction file -x
            case "KeyboardMouse":
                Controller = EController.KeyboardMouse;
                break;
            case "Dualsense":
                Controller = EController.Dualsense;
                break;
            case "Xbox":
                Controller = EController.Xbox;
                break;
            default:
                Controller = EController.Xbox;
                Debug.LogWarning($"[PlayerMovement]: Controller of type {o.currentControlScheme} not found. " + 
                    "Xbox prompts will appear.");
                break;
        }
    }

    // NOTE: All Actions MUST be enabled AND disabled or code will explode (not joking) -x
    // Please enable them individually to avoid errors
    public void OnEnable()
    {
#if UNITY_EDITOR
        _input.Debug.Debug.Enable();
        _input.Debug.Restart.Enable();
#endif
        _input.Gameplay.Movement.Enable();
        _input.Gameplay.Aim.Enable();
        _input.Gameplay.AimMouse.Enable();
        _input.Gameplay.Pause.Enable();
        _input.Gameplay.ReturnToMainMenu.Enable();

        _weaponManager.EnableInput();
    }
    
    public void OnDisable()
    {
        //Reset Movement curves variables
        movementController.OnDisable();
#if UNITY_EDITOR
        _input.Debug.Debug.Disable();   
        _input.Debug.Restart.Disable();
#endif
        _input.Gameplay.Movement.Disable();
        _input.Gameplay.Aim.Disable();
        _input.Gameplay.AimMouse.Disable();
        _input.Gameplay.Pause.Disable();
        _input.Gameplay.ReturnToMainMenu.Disable();

        _weaponManager.DisableInput();

    }

    private void Update()
    {
        // Movement stuff
        bool movingX = _inputDir.x != 0;
        bool movingY = _inputDir.y != 0;
        if (movingX)
            _directionTarget.x = _inputDir.x;
        else 
            _directionTarget.x = 0;

        if (movingY)
            _directionTarget.y = _inputDir.y;
        else
            _directionTarget.y = 0;

        _directionSmooth = 
            Vector2.SmoothDamp(_directionSmooth, _directionTarget, ref _velocity, inputSmoothTime);
        
        movementController.Update(movingX || movingY);
        
        Vector2 moveDirection = Vector2.right * _directionSmooth.x + Vector2.up * _directionSmooth.y;
        Vector2 moveVelocity = moveDirection * movementController.Speed;
        
        _rb.velocity = moveVelocity;
        
        // Aim stuff
        _dir = GetAimWorldPosition();
        _currentDir = Vector3.Slerp(_currentDir, _dir.normalized, aimSmoothTime * Time.deltaTime);
        float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

#if UNITY_EDITOR
        if (debugSpeed)
        {
            currentSpeed = moveVelocity.magnitude;
            Debug.Log(currentSpeed);
        }
        
        if (debugInput) Debug.Log(_inputDir);
#endif
        
        // Animation stuff

        if (moveVelocity.magnitude > 0.1f)
        {
            angle = Mathf.Atan2(moveVelocity.y, moveVelocity.x) * Mathf.Rad2Deg;
            footTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            footAnimation.SetFloat(AnimRate, 1);
            playerAnimation.SetBool(IsIdle, false);
        }
        else
        {
            footAnimation.SetFloat(AnimRate, 0);
            playerAnimation.SetBool(IsIdle, true);
        }

    }

    /// <summary>
    /// Gets raw controller data
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnAim(InputAction.CallbackContext context)
    {
        _rawControllerPosition = context.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Gets raw mouse data
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnAimMouse(InputAction.CallbackContext context)
    {
        _rawMousePosition = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Transform raw input data to a point in world coordinates and returns a vector from the player to said point
    /// </summary>
    /// <returns>Vector from player to position</returns>
    private Vector2 GetAimWorldPosition()
    {
        if (Controller == EController.KeyboardMouse)
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(_rawMousePosition);
            // Threshold distance check
            if (mousePosition.magnitude < aimMinimumDistance)
                return Vector2.zero;
            return (mousePosition - (Vector2)transform.position);
        }
        
        Vector2 controllerPosition = _rawControllerPosition + (Vector2)transform.position;
        return (controllerPosition - (Vector2)transform.position);
    }

    /// <summary>
    /// Grabs direction from Move input
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _inputDir = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Logic for the Debug Button
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnDebug(InputAction.CallbackContext context)
    {
        
#if UNITY_EDITOR
        Debug.Log("Debug Pressed " + context.duration);
        SendMessage("OnKill");
#endif
        
    }

    private void OnOpenPause(InputAction.CallbackContext context)
    {
        _pauseMenu.SetActive(true);
    }

    private void ForceRestart(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public enum EController
{
    KeyboardMouse,
    Dualsense,
    Xbox
}
