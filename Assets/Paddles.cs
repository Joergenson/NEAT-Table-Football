using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddles : MonoBehaviour
{
    [SerializeField] private HingeJoint _hingeJoint;
    [SerializeField] private int strength;
    [SerializeField] private int damp;

    private void Awake()
    {
        _hingeJoint = GetComponent<HingeJoint>();
        var limits = new JointLimits
        {
            min = 0,
            max = 45
        };
        _hingeJoint.limits = limits;
    }

    private void Update()
    {
        var j = new JointSpring();
        j.spring = strength;
        j.damper = damp;
        
        
        if (Input.GetKey(KeyCode.Space))
        {
            j.targetPosition = 45;
        }
        else
        {
            j.targetPosition = 0;
        }

        _hingeJoint.spring = j;
    }
}
