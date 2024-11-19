using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the blood splatter effects in the game.
/// </summary>
public class BloodManager : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform splatterTransform;

    public float splatterAngle = 20;
    public float bloodTravelDistance = 3;
    public int ammountOfFloorBlood = 4;
    public int ammountOfWallBlood = 2;
    public float destroyTime = .5f;

    private float _currentMaxTravelDistance;
    private List<RaycastHit2D> _rayHitList = new List<RaycastHit2D>();

    /// <summary>
    /// Removes the blood manager object from the scene.
    /// </summary>
    private void Remove()
    {
        gameObject.SetActive(false);
        ResourceManager.GetBloodManagerPool().Release(gameObject);
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Initializes the blood manager and schedules its removal.
    /// </summary>
    private void OnEnable()
    {
        _currentMaxTravelDistance = bloodTravelDistance;
        Invoke(nameof(Remove), destroyTime);
        
        Invoke(nameof(Blood), 0);
    }

    /// <summary>
    /// Creates blood splatter effects on the floor and wall.
    /// </summary>
    private void Blood()
    {
        CreateFloorBlood();
        CreateWallBlood();
    }

    /// <summary>
    /// Creates blood splatter effects on the wall.
    /// </summary>
    private void CreateWallBlood()
    {
        for (int i = 0; i < ammountOfWallBlood; i++)
        {
            float randAngle = Random.Range(-splatterAngle, splatterAngle);
            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);

            _rayHitList.Clear();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), _rayHitList, bloodTravelDistance);

            foreach (var hit in _rayHitList)
            {
                int layer = hit.transform.gameObject.layer;
                if (layer == 9 || layer == 10 || layer == 3) continue;

                GameObject wallB = ResourceManager.GetBloodPool().Get();
                wallB.SetActive(true);
                wallB.transform.position = hit.point;

                SpriteRenderer sprW = wallB.GetComponent<SpriteRenderer>();
                sprW.sortingOrder = 20;
                sprW.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                wallB.AddComponent<FloorSplatter>();
                break;
            }
        }
    }

    /// <summary>
    /// Creates blood splatter effects on the floor.
    /// </summary>
    private void CreateFloorBlood()
    {
        for (int k = 0; k < ammountOfFloorBlood; k++)
        {
            float randAngle = Random.Range(-splatterAngle, splatterAngle);
            splatterTransform.localEulerAngles = new Vector3(0, 0, randAngle);

            _rayHitList.Clear();
            int amountHits = Physics2D.Raycast(splatterTransform.position, splatterTransform.right, new ContactFilter2D(), _rayHitList, bloodTravelDistance);

            foreach (var hit in _rayHitList)
            {
                int layer = hit.transform.gameObject.layer;
                if (layer == 9 || layer == 10 || layer == 3) continue;

                _currentMaxTravelDistance = hit.distance;
                break;
            }

            Vector3 randLocInRange = splatterTransform.right * Random.Range(0, _currentMaxTravelDistance);
            GameObject floorB = ResourceManager.GetBloodPool().Get();
            floorB.SetActive(true);
            floorB.transform.position = randLocInRange + splatterTransform.position;

            SpriteRenderer sprW = floorB.GetComponent<SpriteRenderer>();
            sprW.sortingOrder = -2;
            sprW.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            floorB.AddComponent<FloorSplatter>();

            _currentMaxTravelDistance = bloodTravelDistance;
        }
    }
}