using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject spawnPoint;
    [SerializeField] private Ball ballPrefab;
    private Ball _ballInstance;
    public Ball BallInstance
    {
        get
        {
            if (_ballInstance == null)
            {
                InstantiateBall();
            }

            return _ballInstance;
        }
    }

    private void InstantiateBall()
    {
        _ballInstance = Instantiate(ballPrefab,transform);
        _ballInstance.ResetBallPositionAndVelocity();
    }
}
