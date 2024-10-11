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
    private Vector2 _inputDir;
    private Vector2 _directionTarget;
    private Vector2 _directionSmooth;
    private Vector2 _velocity;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugSpeed;
    [SerializeField] private bool debugInput;
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
        //Reset Movement curves variables
        movementController.OnDisable();

        _input.Gameplay.Debug.Disable();   
        _input.Gameplay.Movement.Disable();
    }
    
    void Update()
    {
        // Movement stuff
        // _isInput = _inputDir != Vector2.zero;
        bool isMovingx = _inputDir.x != 0;
        bool isMovingy = _inputDir.y != 0;
        if (isMovingx)
            _directionTarget.x = _inputDir.x;
        else 
            _directionTarget.x = 0;

        if (isMovingy)
            _directionTarget.y = _inputDir.y;
        else
            _directionTarget.y = 0;

        _directionSmooth = 
            Vector2.SmoothDamp(_directionSmooth, _directionTarget, ref _velocity, inputSmoothTime);
        
        movementController.Update(isMovingx || isMovingy);
        
        Vector2 moveDirection = Vector2.right * _directionSmooth.x + Vector2.up * _directionSmooth.y;
        Vector2 moveVelocity = moveDirection * movementController.Speed;
        
        _rigidbody.velocity = moveVelocity;

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
#endif
        
    }
}
