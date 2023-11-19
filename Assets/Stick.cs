using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint joint;
    [SerializeField] private Rigidbody rb;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
    }
    
    public void MoveStick(float amount)
    {
        rb.AddRelativeForce(Vector3.right * (amount * 1f), ForceMode.VelocityChange);
    }

    public void RotateStick(float amount)
    {
        rb.AddRelativeTorque(Vector3.right * (amount * 25), ForceMode.VelocityChange);
    }

    public float GetStickPosition()
    {
        return transform.position.x;
    }
}
