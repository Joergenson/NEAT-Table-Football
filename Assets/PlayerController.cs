using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;

public class PlayerController : UnitController
{
    protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
    {
        throw new System.NotImplementedException();
    }

    protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
    {
        throw new System.NotImplementedException();
    }

    public override float GetFitness()
    {
        return 0; // defined by if it wins 
    }

    protected override void HandleIsActiveChanged(bool newIsActive)
    {
        throw new System.NotImplementedException();
    }
}
