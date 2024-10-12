using UnityEngine;
public class MeleeDamage : MonoBehaviour
{
     public GameObject meleeCollider;
     public float attackTime;
     private BoxCollider2D _collider;
     private SpriteRenderer _renderer;
     private float _time;
     private void Start()
     {
          meleeCollider = GameObject.Find("collider");
          _collider = GetComponent<BoxCollider2D>();
          _renderer = GetComponent<SpriteRenderer>();
     }

     private void Attack()
     {
          _time = 0.0f;
          _renderer.enabled = true;
          _collider.enabled = true;
     }

     void Update()
     {
          _time += Time.deltaTime;
          if (_time >= attackTime)
          {
               _renderer.enabled = false;
               _collider.enabled = false;
          }
     }
     /// <summary>
     /// When it collides with an enemy it sends a message that does damage
     /// </summary>
     /// <param name="collision"></param>
     private void OnCollisionEnter2D(Collision2D collision)
     {
          if (collision.gameObject.tag == "Enemy")
          {
               gameObject.SendMessage("DoMeleeDamage");
          }
     }
}

