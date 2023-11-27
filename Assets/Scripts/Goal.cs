using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private int _goals;

    private void OnTriggerEnter(Collider other)
    {
        Ball comp = other.gameObject.GetComponent<Ball>();
        if (comp != null)
        {
            comp.ResetBallPositionAndVelocity();
            _goals++;
        }
    }

    public int GetGoals()
    {
        return _goals;
    }
}
