using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    public float speed = 2f;

    private bool movingToEnd = true;
    
    void Update()
    {
        float distance = Vector3.Distance(startPosition.position, endPosition.position);
        float totalTime = distance / speed;

        float t = Mathf.PingPong(Time.realtimeSinceStartup / totalTime, 1f);

        transform.position = Vector3.Lerp(startPosition.position, endPosition.position, t);
    }
}
