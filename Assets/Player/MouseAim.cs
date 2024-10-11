using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseAim : MonoBehaviour
{
    PlayerIA _Input;
    [SerializeField] Camera _Camera;
    Vector2 _MousePos;
    Rigidbody2D _Rigidbody;


    private void Awake()
    {
        _Input = new PlayerIA();
        _Rigidbody = GetComponent<Rigidbody2D>();
        _Camera = Camera.main;
    }

    //Actions enabled and disabled to prevent crashes
    private void OnEnable()
    {
        _Input.Enable();
        _Input.Gameplay.Aim.performed += OnAim;
        _Input.Gameplay.Aim.canceled += OnAim;
    }

    private void OnDisable()
    {
        _Input.Disable();
    }
    private void OnAim (InputAction.CallbackContext context)
    {
        //Gets the mouse position in comparison with the camera
        _MousePos = _Camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    
    private void FixedUpdate()
    {
        //Updates the vector of the direction the player is facing
        Vector2 playerDirection = _MousePos - _Rigidbody.position;
        //Calculates the angle needed for that movement and turns the player into the new direction
        float angle = Mathf.Atan2 (playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        _Rigidbody.MoveRotation(angle);
    }
}
