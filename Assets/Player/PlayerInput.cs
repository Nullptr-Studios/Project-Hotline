using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerIA _input;
    private Rigidbody2D _rigidbody;
    
    [Header("Player Movement")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private MotionController movementController;
    [SerializeField] private float inputSmoothTime = 0.2f;
    private Vector2 _direction;
    private Vector2 _directionTarget;
    private Vector2 _directionSmooth;
    private Vector2 _velocity;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugSpeed;
#endif

    private void Awake()
    {
        _input = new PlayerIA();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // NOTE: All Actions MUST be enabled AND disabled or code will explode (not joking) -x
    private void OnEnable()
    {
        _input.Gameplay.Debug.performed += OnDebug;
        _input.Gameplay.Debug.Enable();
        
        _input.Gameplay.Movement.performed += OnMove;
        _input.Gameplay.Movement.canceled += OnMove;
        _input.Gameplay.Movement.Enable();
    }
    
    private void OnDisable()
    {
        _input.Gameplay.Debug.Disable();   
        _input.Gameplay.Movement.Disable();
    }
    
    void Update()
    {
        // Movement stuff
        // _isInput = _direction != Vector2.zero;
        var isMoving = _direction != Vector2.zero;
        if(isMoving)
            _directionTarget = _direction;
        
        _directionSmooth = 
            Vector2.SmoothDamp(_directionSmooth, _directionTarget, ref _velocity, inputSmoothTime);
        
        movementController.Update(isMoving);
        
        var moveDirection = Vector3.right * _directionSmooth.x + Vector3.up * _directionSmooth.y;
        var moveVelocity = moveDirection * movementController.Speed;
        currentSpeed = moveVelocity.magnitude;
        _rigidbody.velocity = moveVelocity;
        
        if (debugSpeed) Debug.Log(currentSpeed);
    }

    /// <summary>
    /// Grabs direction from Move input
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Logic for the Debug Button
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnDebug(InputAction.CallbackContext context)
    {
        
#if UNITY_EDITOR
        Debug.Log("Debug Pressed " + context.duration);
#endif
        
    }
}
