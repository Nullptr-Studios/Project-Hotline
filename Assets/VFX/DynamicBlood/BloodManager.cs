using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BloodManager : MonoBehaviour
{
    [Header("Main Settings")] 
    public Transform splatterTransform;

    public GameObject wallBloodGM;
    public GameObject floorBloodGM;
    
    public float splatterAngle = 20;

    public float bloodTravelDistance = 3;

    private float currentMaxTravelDistance;

    public int ammountOfFloorBlood = 4;
    public int ammountOfWallBlood = 2;

    public float destroyTime = 1.0f;

    private void Remove()
    {
        Destroy(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentMaxTravelDistance = bloodTravelDistance;
        
        Invoke("Remove", destroyTime);

        //Wall raycasts
        for (int i = 0; i < ammountOfWallBlood; i++)
        {
            float randAngle = Random.Range(-splatterAngle,
                splatterAngle);

            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);
            
            List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), rayHitList, bloodTravelDistance);
            
            //this foreach will ignore the player and enemy layer
            foreach (var hit in rayHitList)
            {
                int layer = hit.transform.gameObject.layer;

                if (layer == 9 || layer == 10)
                    continue;

                GameObject wallB =  Instantiate(wallBloodGM, hit.point, new Quaternion());

                wallB.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));

                //avoid further progression in loop
                break;
            }

        }
        
        //Floor blood logic
        for (int k = 0; k < ammountOfFloorBlood; k++)
        {
            float randAngle = Random.Range(-splatterAngle,
                splatterAngle);

            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);

            List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), rayHitList, bloodTravelDistance);

            //this foreach will ignore the player and enemy layer
            foreach (var hit in rayHitList)
            {
                int layer = hit.transform.gameObject.layer;

                if (layer == 9 || layer == 10)
                    continue;

                currentMaxTravelDistance = hit.distance;
                break;
            }


            Vector3 randLocInRange = splatterTransform.right * UnityEngine.Random.Range(currentMaxTravelDistance * .15f, currentMaxTravelDistance);
            
            GameObject floorB =  Instantiate(floorBloodGM, splatterTransform.position + randLocInRange, new Quaternion());

            floorB.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
            
        }
        
        
    }
}
