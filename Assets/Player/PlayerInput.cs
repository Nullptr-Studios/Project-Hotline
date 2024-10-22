using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerInput : MonoBehaviour
{
    private PlayerIA _input;
    private Rigidbody2D _rb;
    private Camera _camera;
    
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
    [Tooltip("This makes the player rotate from its center rather than from its collider")]
    [SerializeField] private bool pivotRotation;
    private bool _usingMouse;
    private Vector2 _rawAimPosition;
    private Vector3 _currentDir;
    private Vector3 _dir;

    private PlayerInput _playerI;

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
        
        _input.Gameplay.Debug.performed += OnDebug;
        _input.Gameplay.Movement.performed += OnMove;
        _input.Gameplay.Movement.canceled += OnMove;
        _input.Gameplay.Aim.performed += OnAim;
        // _input.Gameplay.Aim.canceled += OnAim;

        _playerI = GetComponent<PlayerInput>();
    }

    // NOTE: All Actions MUST be enabled AND disabled or code will explode (not joking) -x
    // Please enable them individually to avoid errors
    public void OnEnable()
    {
        _input.Gameplay.Debug.Enable();
        _input.Gameplay.Movement.Enable();
        _input.Gameplay.Aim.Enable();
    }
    
    public void OnDisable()
    {
        //Reset Movement curves variables
        movementController.OnDisable();

        _input.Gameplay.Debug.Disable();   
        _input.Gameplay.Movement.Disable();
        _input.Gameplay.Aim.Disable();
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
        _currentDir = Vector3.Slerp(_currentDir, _dir, aimSmoothTime * Time.deltaTime);
        float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
        _rb.MoveRotation(angle);

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
    /// Gets raw mouse and controller data
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnAim(InputAction.CallbackContext context)
    {
        // TODO: Find a better way to do this
        _usingMouse = context.control.layout.Equals("Vector2");
        
        _rawAimPosition = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Transform raw input data to a point in world coordinates and returns a vector from the player to said point
    /// </summary>
    /// <returns>Vector from player to position</returns>
    private Vector2 GetAimWorldPosition()
    {
        if (_usingMouse)
        {
            Vector2 aimPosition = _camera.ScreenToWorldPoint(_rawAimPosition);
            //Updates the vector of the direction the player is facing
            return (aimPosition - (Vector2)transform.position).normalized;
        }
        
        Vector2 controllerPosition = _rawAimPosition + (Vector2)transform.position;
        return (controllerPosition - (Vector2)transform.position).normalized;
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
