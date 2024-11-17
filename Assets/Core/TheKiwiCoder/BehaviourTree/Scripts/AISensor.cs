using TheKiwiCoder;
using UnityEngine;
using UnityEngine.Serialization;

public class AISensor : MonoBehaviour
{
    public float range = 10;
    public float nearDetectionRange = 1;
    public float angle = 45;
    public int scanFrequency = 15; // Frequency is with q uwu -x
    
    public LayerMask layerToSee;
    public LayerMask layerToOcclude;

    public GameObject detectedPlayer;
    public Vector3 fallbackPosition = Vector3.zero;
    
    public Vector2 heardPosition;

    public bool isDeaf;
    public bool heardPlayer;
    public bool isDetecting;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color meshColor = Color.red;
    [SerializeField] private Color detectColor = Color.white;
    [SerializeField] private bool logVisionHit;
#endif
    
    private Collider2D[] _colliders = new Collider2D[50];
    private int _count;
    private float _scanInterval;
    private float _scanTimer;

    private Transform _trans;
    
    private Vector3 _pPos;
    
    private Mesh _mesh;
    // Start is called before the first frame update
    void Start()
    {
        _scanInterval = 1.0f / scanFrequency;
        _trans = transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer <= 0)
        {
            _scanTimer += _scanInterval;
            Scan();
        }
    }

    public Vector3 GetPlayerPositionIfInBounds()
    {
        return _pPos;
    }
    
    public void HeardPlayer(Vector3 transformPosition)
    {
        if (isDeaf) return;
        
        heardPlayer = true;
        heardPosition = transformPosition;
    }

    private void Scan()
    {
        _count = Physics2D.OverlapCircleNonAlloc(_trans.position, range, _colliders, layerToSee);
        
        //This has been changed from a list to an array in order to optimize code
        isDetecting = false;
        detectedPlayer = null;       
        
        if(_count == 0)
        {
            _pPos = Vector3.zero;
            return;
        }
        
        for (int i = 0; i < _count; ++i)
        {
            GameObject obj = _colliders[i].gameObject;
            _pPos = obj.transform.position;
            if (IsInSight(obj))
            {
                Vector3 origin = _trans.position;
                Vector3 dest = obj.transform.position;
                Vector3 dir = dest - origin;
                
                if(dir.sqrMagnitude < nearDetectionRange)
                {
                    //do fallback
                    fallbackPosition = obj.transform.position + dir.normalized * (-1 * (nearDetectionRange + 2));
                    #if UNITY_EDITOR
                    Debug.DrawLine(transform.position, fallbackPosition, Color.blue, 1);
                    #endif
                }
                else
                {
                    fallbackPosition = Vector3.zero;
                }
                
                PlayerHealth ph = obj.GetComponent<PlayerHealth>();
                if (ph && ph.IsDead)
                {
                    detectedPlayer = null;
                    isDetecting = false;

                    //Disable tree
                    GetComponent<BehaviourTreeRunner>().enabled = false;
                    GetComponent<EnemyWeaponManager>().useWeapon(false);

                    this.enabled = false;
                    
                    break;
                }
                
                detectedPlayer = obj;
                isDetecting = true;
                
                //stop iterating the loop
                break;
            }
        }
    }

    public bool IsInSight(GameObject target)
    {
        
        Vector3 origin = _trans.position;
        Vector3 dest = target.transform.position;
        Vector3 dir = dest - origin;

        if (dir.sqrMagnitude < nearDetectionRange)
        {
            return true;
        }
        
        float deltaAngle = Vector3.Angle(dir, _trans.up);

        if (deltaAngle > angle)
            return false;

        if (Physics2D.Linecast(origin, dest, layerToOcclude))
            return false;

        return true;
        
    }
    
    

#if UNITY_EDITOR
        
    //not used anymore
    private void DrawLines(bool result, Vector3 origin, Vector3 dest, Vector3 dir)
    { 
        if (logVisionHit) 
            Debug.Log(Physics2D.Linecast(origin, dest, layerToOcclude).collider?.gameObject.name);
        
        if(drawGizmos)
            Debug.DrawRay(origin, dir, result ? Color.green : Color.red, .05f);
    }
    
    Mesh CreateMesh()
    {
        Mesh gizmoMesh = new Mesh(); // renamed to avoid occlude warning, was hard containing myself to use var -x

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;
        
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, 0, -angle) * Vector3.up * range;
        Vector3 bottomRight = Quaternion.Euler(0, 0, angle) * Vector3.up * range;
        
        //i´ll add depth to just see the _mesh
        Vector3 topCenter = bottomCenter + Vector3.forward * 1;
        Vector3 topRight = bottomRight + Vector3.forward * 1;
        Vector3 topLeft = bottomLeft + Vector3.forward * 1;

        int vert = 0;
        
        //left
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        
        //right
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;
        
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;
        
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, 0, currentAngle) * Vector3.up * range;
            bottomRight = Quaternion.Euler(0, 0, currentAngle + deltaAngle) * Vector3.up * range;
            
            topRight = bottomRight + Vector3.forward * 1;
            topLeft = bottomLeft + Vector3.forward * 1;
            
            //far
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
        
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
        
            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
        
            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
            
            currentAngle += deltaAngle;
        }


        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }
        
        gizmoMesh.vertices = vertices;
        gizmoMesh.triangles = triangles;
        gizmoMesh.RecalculateNormals();
        
        Vector3[] normals = gizmoMesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        gizmoMesh.normals = normals;

        for (int m = 0; m < gizmoMesh.subMeshCount; m++)
        {
            int[] triangles2 = gizmoMesh.GetTriangles(m);
            for (int i = 0; i < triangles2.Length; i += 3)
            {
                (triangles2[i + 0], triangles2[i + 1]) = (triangles2[i + 1], triangles2[i + 0]);
            }
            gizmoMesh.SetTriangles(triangles2, m);
        }
        
        
        return gizmoMesh;
    }

    private void OnValidate()
    {
        _mesh = CreateMesh();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (_mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, Vector3.one);
            }

            Gizmos.DrawWireSphere(transform.position, range);
            for (int i = 0; i < _count; ++i)
            {
                Gizmos.DrawSphere(_colliders[i].transform.position, .4f);
            }
            
            Gizmos.DrawSphere(transform.position, nearDetectionRange);

            Gizmos.color = detectColor;
            if(detectedPlayer != null)
            {
                Gizmos.DrawSphere(detectedPlayer.transform.position, .4f);

            }
        }
    }
    
#endif
}
