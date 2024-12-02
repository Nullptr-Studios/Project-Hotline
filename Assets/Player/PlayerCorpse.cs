using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the behavior of the player's corpse, including adding force and handling the death screen.
/// </summary>
public class PlayerCorpse : MonoBehaviour
{
    /// <summary>
    /// List of sprites for the player's corpse.
    /// </summary>
    public List<Sprite> playerCorpseSprites;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider2D;
    private CorpseConfig _corpseConfig;
    private float _timer = 0.0f;
    private Canvas deathScreenUI;
    private PixelPerfectCamera camera;
    private PlayerIA _input;
    
    [SerializeField] private float screenDeathTimer = .5f;

    /// <summary>
    /// Adds force to the player's corpse in a specified direction and initializes the death screen UI and camera.
    /// </summary>
    /// <param name="dir">The direction to apply the force.</param>
    /// <param name="deathScreenUI">The canvas for the death screen UI.</param>
    /// <param name="camera">The pixel perfect camera.</param>
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

    /// <summary>
    /// Restarts the game when the accept input action is performed.
    /// </summary>
    /// <param name="obj">The input action callback context.</param>
    private void RestartGame(InputAction.CallbackContext obj)
    {
        _input.UI.Accept.performed -= RestartGame;
        _input.UI.Accept.Disable();
        VariableRepo.Instance.RemoveAll();

        
        SceneMng.Reload();
    }

    /// <summary>
    /// Opens the death screen UI.
    /// </summary>
    private void OpenDeathScreen()
    {
        // Have to deactivate pixel perfect camera as it disables antialiasing so the shader won't work
        // TODO: Look y the shader doesnt work w/o antialiasing
        camera.enabled = false;
        _input.UI.Accept.Enable();
        deathScreenUI.gameObject.SetActive(true);
        
        
        
    }

    /// <summary>
    /// Updates the state of the player's corpse, including handling the timer and opening the death screen.
    /// </summary>
    void Update()
    {
        if (_rb)
        {
            if (_timer >= screenDeathTimer)
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