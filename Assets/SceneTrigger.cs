using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTrigger : MonoBehaviour
{
    public UnityEvent triggerEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
            triggerEvent.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawArrow(transform.position, transform.position + Vector3.right);
    }
    
    /// <summary>
    /// arrow drawing debug
    /// </summary>
    Vector2 _arrowPos;
    Vector2 _arrowDirection;
    readonly Vector3 angleVectorUp=new Vector3(0f, 0.40f,-1f)*0.2f/*length*/;
    readonly Vector3 angleVectorDown=new Vector3(0f, -0.40f,-1f)*0.2f/*length*/;
    Vector2 _upTmp;
    Vector2 _downTmp;
    private void DrawArrow(Vector2 startPos, Vector2 endPos)
    {
        _arrowDirection=endPos - startPos;
        _arrowPos = startPos + (_arrowDirection*0.9f/*position along line*/);

        _upTmp = Quaternion.LookRotation(_arrowDirection) * angleVectorUp ;
        _downTmp = Quaternion.LookRotation(_arrowDirection) * angleVectorDown;

        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawRay(_arrowPos, _upTmp);
        Gizmos.DrawRay(_arrowPos, _downTmp);
    }
}