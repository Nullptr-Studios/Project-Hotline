using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickering : MonoBehaviour
{
    private Light2D _light;

    private int frames = 0;

    [SerializeField] private int framesPerRandomize;

    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        if (frames % framesPerRandomize == 0)
        {
            RandomizeIntensity();
        }
    }

    void RandomizeIntensity()
    {
        // Create an instance of the Random class
        System.Random random = new System.Random();
        float randomValue = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        _light.intensity = randomValue;
    }
}
