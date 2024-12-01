using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PC : MonoBehaviour
{
    private Animator _animator;
    
    private SpriteRenderer _spriteRenderer;
    private PlayerIA _input;
    
    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _input = new PlayerIA();
        _input.UI.Accept.performed += ctx => Accept();
        _input.UI.Accept.Enable();
    }

    private void OnDestroy()
    {
        _input.UI.Accept.performed -= ctx => Accept();
        _input.UI.Accept.Disable();
        _input.Dispose();
    }

    private void cac()
    {
        SceneManager.LoadScene("MainMenu2");
    }

    private void Accept()
    {
        _animator.SetTrigger("Insert");
        Invoke("cac", 1);
    }
}
