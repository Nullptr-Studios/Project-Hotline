using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    private Rigidbody2D _rb;
    //private BoxCollider2D _collider2D;

    private CorpseConfig _corpseConfig;

    private float _timer = 0.0f;
    // Start is called before the first frame update

    public void CorpseAddForceInDir(Vector2 dir)
    {
        _rb = gameObject.AddComponent<Rigidbody2D>();
        //_collider2D = gameObject.AddComponent<BoxCollider2D>();
        gameObject.layer = 9;

        _corpseConfig = ResourceManager.GetCorpseConfig();

        _rb.drag = _corpseConfig.Drag;
        _rb.gravityScale = 0;
        
        _rb.freezeRotation = true;
        
        _rb.AddForce(dir * _corpseConfig.Force);

        transform.right = dir;
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb)
        {
            if (_timer >= 2)
            {
                Destroy(_rb);
                //Destroy(_collider2D);
                Destroy(this);
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }
}
