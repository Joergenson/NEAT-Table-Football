using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    private ConfigurableJoint _joint;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MoveStick(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MoveStick(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RotateStick(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RotateStick(-1);
        }
    }

    void MoveStick(float amount)
    {
        _rb.AddRelativeForce(Vector3.right * (amount * 1f), ForceMode.VelocityChange);
    }

    void RotateStick(float amount)
    {
        _rb.AddRelativeTorque(Vector3.right * (amount * 25f), ForceMode.VelocityChange);

    }
}
