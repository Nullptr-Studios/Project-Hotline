using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_BarikDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private float move;
    [SerializeField] private float speed;

    private Collider2D _collider;
    private bool _isPlayer;
    private Vector3 _startPosition;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _startPosition = door.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayer && !Mathf.Approximately(door.transform.position.x + move, 0))
            transform.position = new Vector3(
                door.transform.position.x + Time.deltaTime * speed * Mathf.Sign(move),
                door.transform.position.y,
                door.transform.position.z
            );
        else if (!_isPlayer && !Mathf.Approximately(door.transform.position.x, _startPosition.x))
            transform.position = new Vector3(
                door.transform.position.x - Time.deltaTime * speed * Mathf.Sign(move),
                door.transform.position.y,
                door.transform.position.z
            );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _isPlayer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _isPlayer = false;
    }
}
