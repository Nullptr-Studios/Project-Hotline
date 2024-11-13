using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Structure to hold camera variables such as max rotation and max distance.
/// </summary>
[System.Serializable]
public struct SCameraVariables
{
    public float maxRotation;
    public float maxDistance;
    public Vector2 Origin;
}

/// <summary>
/// Controls the camera behavior based on the player's weapon type and position.
/// </summary>
public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _vcamera;
    private GameObject _player;
    
    private PlayerWeaponManager _playerWeaponManager;
    
    /// <summary>
    /// Default view distance from the player.
    /// </summary>
    public float defaultPlayerCameraDistance = 0;
    /// <summary>
    /// View distance from the player when using melee weapons.
    /// </summary>
    public float meleePlayerCameraDistance = 1.5f;
    /// <summary>
    /// View distance from the player when using ranged weapons.
    /// </summary>
    public float rangedPlayerCameraDistance = 2.5f;

    [SerializeField] private float _currentPlayerCameraDistance;

    private GameObject _lookGameObject;

    private float _baseRotation;
    private SCameraVariables _cameraVariables;

    private Vector3 playerLookPos = new Vector3();

    /// <summary>
    /// Initializes the camera controller.
    /// </summary>
    void Awake()
    {
        _vcamera = GetComponent<CinemachineVirtualCamera>();
        _baseRotation = _vcamera.transform.eulerAngles.z;
        _player = GameObject.FindGameObjectWithTag("Player");
        
        _playerWeaponManager = _player.GetComponent<PlayerWeaponManager>();
        
        _currentPlayerCameraDistance = defaultPlayerCameraDistance;

        _lookGameObject = Instantiate(new GameObject());
        _lookGameObject.transform.position = _player.transform.position;
        _vcamera.Follow = _lookGameObject.transform;
    }

    /// <summary>
    /// Updates the camera position and rotation based on the player's weapon type and position.
    /// </summary>
    void Update()
    {
        // Adjust the camera distance based on the current weapon type
        switch (_playerWeaponManager.GetCurrentWeaponType())
        {
            case EWeaponType.Default:
                _currentPlayerCameraDistance = defaultPlayerCameraDistance;
                break;
            case EWeaponType.Melee:
                _currentPlayerCameraDistance = meleePlayerCameraDistance;
                break;
            case EWeaponType.Fire:
                _currentPlayerCameraDistance = rangedPlayerCameraDistance;
                break;
        }

        // Adjust the follow position based on the direction
        playerLookPos = _player.transform.position + (_player.transform.right * _currentPlayerCameraDistance);
        _lookGameObject.transform.position = playerLookPos;

        // Retrieve the active scene camera variables
        _cameraVariables = SceneMng.ActiveSceneCameraVars;

        var distance = _player.transform.position.x - _cameraVariables.Origin.x;
        var rotation = _cameraVariables.maxRotation * Mathf.Clamp((distance / _cameraVariables.maxDistance), -1, 1) + _baseRotation;
        _vcamera.transform.eulerAngles = new Vector3(0f, 0f, rotation);
    }
    
#if UNITY_EDITOR

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private float lineLength;

    /// <summary>
    /// Draws debug gizmos in the editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        // World divider gizmo
        var startPos = new Vector3(_cameraVariables.Origin.x, _cameraVariables.Origin.y + lineLength/2, 0);
        var endPos = new Vector3(_cameraVariables.Origin.x, _cameraVariables.Origin.y - lineLength/2, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, endPos);

        // Max rotation gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_cameraVariables.Origin, new Vector3(_cameraVariables.maxDistance*2, 1, 1));
        
        Gizmos.DrawWireCube(playerLookPos, new Vector3(1, 1, 1));
    }

#endif
}