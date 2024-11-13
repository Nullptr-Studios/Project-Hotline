using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the emission of particles and manages their lifecycle.
/// </summary>
public class EmmiterHandler : MonoBehaviour
{
    /// <summary>
    /// The time after which the emitter will be disabled.
    /// </summary>
    public float time;

    /// <summary>
    /// The index of the emitter, used to determine the type of particle pool.
    /// </summary>
    public int emmiterIndex;

    /// <summary>
    /// The list of particle systems to be managed by this handler.
    /// </summary>
    public List<ParticleSystem> particles;

    /// <summary>
    /// Cached reference to the GameObject.
    /// </summary>
    private GameObject _gameObject;

    /// <summary>
    /// Caches the GameObject reference.
    /// </summary>
    private void Awake()
    {
        _gameObject = gameObject;
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Starts all particle systems and schedules the emitter for disabling.
    /// </summary>
    public void OnEnable()
    {
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        Invoke(nameof(Delete), time);
    }

    /// <summary>
    /// Disables the GameObject and releases it back to the appropriate pool.
    /// </summary>
    private void Delete()
    {
        _gameObject.SetActive(false);
        var pool = emmiterIndex switch
        {
            6 => ResourceManager.GetWallHitPool(),
            7 => ResourceManager.GetGlassHitPool(),
            8 => ResourceManager.GetWallBangHitPool(),
            9 => ResourceManager.GetMuzzlePool(),
            _ => null
        };

        pool?.Release(_gameObject);
    }
}