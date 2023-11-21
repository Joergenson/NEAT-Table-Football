using System;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;

public class PlayerController : UnitController
{
    [SerializeField] private Stick stick;

    protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
    {
        inputSignalArray[0] = stick.GetStickPosition();
    }

    protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
    {
        var m = (float)outputSignalArray[0];
        stick.MoveStick(m);
    }

    public override float GetFitness()
    {
        return 0; // defined by if it wins 
    }

    protected override void HandleIsActiveChanged(bool newIsActive)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(newIsActive);
        }
    }
}
