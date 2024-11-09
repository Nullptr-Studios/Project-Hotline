using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the blood splatter effects in the game.
/// </summary>
public class BloodManager : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform splatterTransform;

    public float splatterAngle = 20;

    public float bloodTravelDistance = 3;

    private float currentMaxTravelDistance;

    public int ammountOfFloorBlood = 4;
    public int ammountOfWallBlood = 2;

    public float destroyTime = 1.0f;

    /// <summary>
    /// Removes the blood manager object from the scene.
    /// </summary>
    private void Remove()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the blood manager and creates blood splatter effects.
    /// </summary>
    void Start()
    {
        currentMaxTravelDistance = bloodTravelDistance;

        Invoke("Remove", destroyTime);

        // Wall raycasts
        for (int i = 0; i < ammountOfWallBlood; i++)
        {
            float randAngle = Random.Range(-splatterAngle, splatterAngle);

            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);

            List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), rayHitList, bloodTravelDistance);

            // This foreach will ignore the player and enemy layer
            foreach (var hit in rayHitList)
            {
                int layer = hit.transform.gameObject.layer;

                // Ignore
                if (layer == 9 || layer == 10 || layer == 3)
                    continue;

                GameObject wallB = ResourceManager.GetBloodPool().Get();

                wallB.SetActive(true);

                wallB.transform.position = hit.point;

                // Sprite settings
                SpriteRenderer sprW = wallB.GetComponent<SpriteRenderer>();
                sprW.sortingOrder = 2;
                sprW.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                // Script
                wallB.AddComponent<FloorSplatter>();

                // Avoid further progression in loop
                break;
            }
        }

        // Floor blood logic
        for (int k = 0; k < ammountOfFloorBlood; k++)
        {
            float randAngle = Random.Range(-splatterAngle, splatterAngle);

            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);

            List<RaycastHit2D> rayHitList = new List<RaycastHit2D>();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), rayHitList, bloodTravelDistance);

            // This foreach will ignore the player and enemy layer
            foreach (var hit in rayHitList)
            {
                int layer = hit.transform.gameObject.layer;

                // Ignore
                if (layer == 9 || layer == 10 || layer == 3)
                    continue;

                currentMaxTravelDistance = hit.distance;
                break;
            }

            Vector3 randLocInRange = splatterTransform.right * Random.Range(0, currentMaxTravelDistance);

            GameObject floorB = ResourceManager.GetBloodPool().Get();

            floorB.SetActive(true);

            floorB.transform.position = randLocInRange + splatterTransform.position;

            // Sprite settings
            SpriteRenderer sprW = floorB.GetComponent<SpriteRenderer>();
            sprW.sortingOrder = -2;
            sprW.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            // Script
            floorB.AddComponent<FloorSplatter>();
        }
    }
}