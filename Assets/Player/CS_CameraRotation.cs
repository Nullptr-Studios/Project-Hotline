using Cinemachine;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float maxRotation;
    [SerializeField] private float maxDistance;
    private float baseRotation;

    [Header("Components")]
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera vcamera;

    // Start is called before the first frame update
    void Start()
    {
        baseRotation = vcamera.transform.eulerAngles.z;

        if (player == null || vcamera == null)
        {
            Debug.LogWarning($"[Camera Rotation] {this.name}: No reference for player or camera. Disabling camera rotation.");
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var distance = player.transform.position.x - transform.position.x;
        var rotation = maxRotation * Mathf.Clamp((distance / maxDistance), -1, 1) + baseRotation;
        vcamera.transform.eulerAngles = new Vector3(0f, 0f, rotation);
    }

#if UNITY_EDITOR

    [Header("Debug")]
    [SerializeField] private float lineLength;

    private void OnDrawGizmos()
    {
        // World divider gizmo
        var startPos = new Vector3(transform.position.x, transform.position.y + lineLength/2, transform.position.z);
        var endPos = new Vector3(transform.position.x, transform.position.y - lineLength/2, transform.position.z);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, endPos);

        // Max rotation gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(maxDistance*2, 1, 1));
    }

#endif

}
