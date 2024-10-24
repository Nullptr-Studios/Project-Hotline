using UnityEngine;
using UnityEngine.InputSystem;
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
        
        _input.Gameplay.Debug.performed += OnDebug;
        _input.Gameplay.Movement.performed += OnMove;
        _input.Gameplay.Movement.canceled += OnMove;
        _input.Gameplay.Aim.performed += OnAim;
        _input.Gameplay.AimMouse.performed += OnAimMouse;
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
            case "Controller":
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
        _input.Gameplay.Debug.Enable();
        _input.Gameplay.Movement.Enable();
        _input.Gameplay.Aim.Enable();
        _input.Gameplay.AimMouse.Enable();

        _weaponManager.EnableInput();
    }
    
    public void OnDisable()
    {
        //Reset Movement curves variables
        movementController.OnDisable();

        _input.Gameplay.Debug.Disable();   
        _input.Gameplay.Movement.Disable();
        _input.Gameplay.Aim.Disable();
        _input.Gameplay.AimMouse.Disable();

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
}

public enum EController
{
    KeyboardMouse,
    Dualsense,
    Xbox
}
