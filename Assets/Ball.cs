using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector3 resetPos;
    private Rigidbody _rb;
    [SerializeField] private float forceMagnitude;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ResetBallPositionAndVelocity();
        Invoke(nameof(KickBallInRandomDirection),0);
    }

    private void ResetBallPositionAndVelocity()
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
}
