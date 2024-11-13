using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmiterHandler : MonoBehaviour
{
    public float time;
    public int emmiterIndex;

    public List<ParticleSystem> particles;

    // Start is called before the first frame update
    public void OnEnable()
    {
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        Invoke("Delete", time);
    }
    
    private void Delete()
    {
        gameObject.SetActive(false);
        switch (emmiterIndex)
        {
            case 6:
                ResourceManager.GetWallHitPool().Release(gameObject);
                break;
            case 7:
                ResourceManager.GetGlassHitPool().Release(gameObject);
                break;
            case 8:
                ResourceManager.GetWallBangHitPool().Release(gameObject);
                break;
            case 9:
                ResourceManager.GetMuzzlePool().Release(gameObject);
                break;
        }
    }
}
