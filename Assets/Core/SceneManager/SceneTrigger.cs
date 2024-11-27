using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTrigger : MonoBehaviour
{
    public GameObject ExitLoc;

    /// <summary>
    /// this is done in order to call external functions whenever we enter the trigger (loading scenes, unloading scenes, scene logic ...)
    /// </summary>
    public UnityEvent EntranceTriggerEvent;



    private Vector2 _exitPos;
    private Animator _animatorFade;
    
    private bool start = false;
    
    private GameObject other;

    private float _timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _animatorFade = GameObject.Find("ScreenLevelTransition").GetComponent<Animator>();
        _exitPos = ExitLoc.transform.position;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _animatorFade.SetTrigger("In");
            _animatorFade.ResetTrigger("Out");

            this.other = other.gameObject;
            start = true;
        }
    }

    private void Update()
    {
        if (start)
        {
            if(_timer >= .333f)
            {
                EntranceTriggerEvent.Invoke();
                other.transform.position = _exitPos;
                start = false;
                _timer = 0;
                _animatorFade.ResetTrigger("In");
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color boxCol = Color.red;
        boxCol.a = .25f;
        Gizmos.color = boxCol;
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.color = Color.yellow;
        DrawArrow(transform.position, ExitLoc.transform.position);
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
#endif
}