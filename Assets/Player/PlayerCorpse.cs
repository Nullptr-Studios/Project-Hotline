using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCorpse : MonoBehaviour
{
    public List<Sprite> playerCorpseSprites;
    
    private Rigidbody2D _rb;
    private BoxCollider2D _collider2D;

    private CorpseConfig _corpseConfig;

    private float _timer = 0.0f;
    
    private Canvas deathScreenUI;
    private PixelPerfectCamera camera;
    
    private PlayerIA _input;
    // Start is called before the first frame update

    public void CorpseAddForceInDir(Vector2 dir, Canvas deathScreenUI, PixelPerfectCamera camera)
    {
        this.deathScreenUI = deathScreenUI;
        this.camera = camera;
        
        _rb = gameObject.AddComponent<Rigidbody2D>();
        _collider2D = gameObject.AddComponent<BoxCollider2D>();
        gameObject.layer = 9;

        _corpseConfig = ResourceManager.GetCorpseConfig();

        _rb.drag = _corpseConfig.Drag;
        _rb.gravityScale = 0;
        
        _rb.AddForce(dir * _corpseConfig.Force);

        transform.right = dir;
        
        GetComponent<SpriteRenderer>().sprite = playerCorpseSprites[Random.Range(0, playerCorpseSprites.Count)];
        
        _input = new PlayerIA();
        _input.UI.Accept.performed += RestartGame;
    }

    private void RestartGame(InputAction.CallbackContext obj)
    {
        _input.UI.Accept.performed -= RestartGame;
        _input.UI.Accept.Disable();
        VariableRepo.Instance.RemoveAll();
        
        // TODO: this should be fixed after prototype to not have the player read the VN every time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OpenDeathScreen()
    {
        // Have to deactivate pixel perfect camera as it disables antialiasing so the shader won't work
        // TODO: Look y the shader doesnt work w/o antialiasing
        camera.enabled = false;
        _input.UI.Accept.Enable();
        deathScreenUI.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb)
        {
            if (_timer >= 2)
            {
                Destroy(_rb);
                Destroy(_collider2D);
                OpenDeathScreen();
                //Destroy(this);
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }
}