using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;

public class TurnamnetNEATSupervisor : NeatSupervisor
{
    /// <summary>
    /// Instantiates a Unit in case no Unit can be drawn from the _unusedUnitPool.
    /// </summary>
    private UnitController InstantiateUnit(IBlackBox box, bool player1 = true)
    {
        UnitController controller = Instantiate(_unitControllerPrefab, _unitControllerPrefab.transform.position, _unitControllerPrefab.transform.rotation);
        
        controller.transform.parent = _spawnParent != null ? _spawnParent : transform;
        if (!player1)
        {
            controller.transform.Rotate(new Vector3(0, 180, 0));
        }

        _blackBoxMap.Add(box, controller);
        return controller;
    }
    
    /// <summary>
    /// Spawns a Unit. This means either reusing a deactivated unit from the pool or to instantiate a Unit into the pool, in case the pool is empty.
    /// Units don't get Destroyed, instead they are just reset to avoid unneccessary instantiation calls.
    /// </summary>
    private UnitController GetUnusedUnit(IBlackBox box, bool player1 = true)
    {
        UnitController controller;

        if (_unusedUnitsPool.Any())
        {
            controller = _unusedUnitsPool.First();
            _blackBoxMap.Add(box, controller);
        }
        else
        {
            controller = InstantiateUnit(box, player1);
        }

        PoolUnit(controller, true);
        return controller;
    }
    
    /// <summary>
    /// Creates (or re-uses) a UnitController instance and assigns the Neural Net (IBlackBox) to it and activates it, so that it starts executing the Net.
    /// </summary>
    public void ActivateUnit(IBlackBox box, bool player1 = true)
    {
        UnitController controller = GetUnusedUnit(box, player1);
        controller.ActivateUnit(box);
    }
    
    
}
