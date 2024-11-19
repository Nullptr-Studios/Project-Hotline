using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    [SerializeField] private Train leftTrain;
    [SerializeField] private Train rightTrain;
    private Train[] _trains = new Train[2];
    
    // I'm hardcoding this as this system is not going to be reused
    private const float START_POSITION_Y = 45;
    private const float MAX_POSITION_Y = -17;
    
    // Start is called before the first frame update
    void Start()
    {
        _trains[0] = leftTrain;
        _trains[1] = rightTrain;
        foreach (Train tr in _trains)
        {
            var train = tr; // you have to create this to be able to use a struct in a foreach loop -x
            train.timer = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < _trains.Length; i++)
        {
            if (i > 4) break; // Doing this to avoid a loop in case of a rare error -x
            
            if (_trains[i].timer + _trains[i].rate < Time.time)
            {   // Return train to spawn position -x
                _trains[i].@object.transform.position =
                    new Vector3(_trains[i].@object.transform.position.x, START_POSITION_Y, 0f);
                _trains[i].timer = Time.time;
            }
            else if (_trains[i].@object.transform.position.y > MAX_POSITION_Y)
            {   // Update train movement -x
                _trains[i].@object.transform.position = new Vector3(_trains[i].@object.transform.position.x,
                    _trains[i].@object.transform.position.y + _trains[i].speed * Time.deltaTime,
                    0f);
            }
        }
    }

    [System.Serializable]
    private struct Train
    {
        public GameObject @object;
        public float speed;
        public float rate;
        public float timer;
    }
}
