using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public Vector3 resetPos;
    private Rigidbody _rb;
    [SerializeField] private float forceMagnitude;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float timeSinceLastMovement;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ResetBallPositionAndVelocity();
        Invoke(nameof(KickBallInRandomDirection),0);
        
        lastPosition = _rb.position;
        lastRotation = _rb.rotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Top"))
        {
            ResetBallPositionAndVelocity();
        }
    }

    void Update()
    {
        if (transform.position.y < 0)
        {
            ResetBallPositionAndVelocity();
        }
        // Check if the Rigidbody has moved or rotated
        if (HasRigidbodyMoved())
        {
            // If it has moved, update the last position and rotation
            lastPosition = _rb.position;
            lastRotation = _rb.rotation;
            timeSinceLastMovement = 0f;
        }
        else
        {
            // If it hasn't moved, increment the idle time
            timeSinceLastMovement += Time.deltaTime;

            // Check if the idle time has exceeded the threshold
            if (timeSinceLastMovement >= 5)
            {
                ResetBallPositionAndVelocity();
            }
        }
    }

    private bool OutOfTable()
    {
        return Vector3.Distance(Vector3.zero, transform.position) > 300;
    }
    
    private bool HasRigidbodyMoved()
    {
        return Vector3.Distance(lastPosition, _rb.position) > 0.001f || Quaternion.Angle(lastRotation, _rb.rotation) > 0.1f;
    }

    public void ResetBallPositionAndVelocity()
    {
        transform.localPosition = resetPos;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void KickBallInRandomDirection()
    {
        // Generate a random direction in the horizontal plane (excluding vertical axis)
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0f; // Set the y-component to 0 to exclude vertical direction

        // Apply force in the random direction
        Vector3 force = randomDirection * forceMagnitude;
        _rb.AddForce(force, ForceMode.Impulse);
    }

    public Vector3 GetBallPosition()
    {
        return transform.position;
    }

    public Vector3 GetBallVelocity()
    {
        return _rb.velocity;
    }
}
