using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stick : MonoBehaviour
{
    [SerializeField] public ConfigurableJoint joint;
    [SerializeField] private Rigidbody rb;
    private int _interactions;
    [SerializeField] public float rotationTimer;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.localPosition.x > joint.linearLimit.limit)
        {
            transform.localPosition = new Vector3(joint.linearLimit.limit, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.position.x < -joint.linearLimit.limit)
        {
            transform.localPosition = new Vector3(-joint.linearLimit.limit, transform.localPosition.y, transform.localPosition.z);
        }

        bool isRotating = rb.angularVelocity.magnitude > 0.1f;

        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
        }
    }
    
    public void MoveStick(float namount)
    {
        var amount = namount * 2.0f - 1.0f;
        rb.AddRelativeForce(Vector3.right * (amount * 1f), ForceMode.VelocityChange);
    }
    
    public void RotateStick(float namount)
    {
        var amount = namount * 2.0f - 1.0f;
        rb.AddRelativeTorque(Vector3.right * (amount * 25f), ForceMode.VelocityChange);
    }

    public float GetStickPosition()
    {
        return (transform.position.x + joint.linearLimit.limit) / (2 * joint.linearLimit.limit);
    }

    public float GetStickVelocity()
    {
        return Mathf.Clamp01((rb.velocity.x + 3.4f) / (2 * 3.4f));
    }

    public float GetStickAngularVelocity()
    {
        return Mathf.Clamp01((rb.angularVelocity.x + 25f) / 50f);
    }

    public void AddInteraction()
    {
        _interactions++;
    }

    public int GetInteractions()
    {
        return _interactions;
    }
}
