using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;

public class TurnamnetNEATSupervisor : NeatSupervisor
{
    [SerializeField] private PlayerController playerControllerPrefab;
    
    /// <summary>
    /// Instantiates a Unit in case no Unit can be drawn from the _unusedUnitPool.
    /// </summary>
    private UnitController InstantiateUnit(IBlackBox box, int table, bool player1 = true)
    {
        PlayerController controller = Instantiate(playerControllerPrefab);
        
        MoveTable(controller, table);
        controller.ball = _tables[table].BallInstance;
        
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
    private UnitController GetUnusedUnit(IBlackBox box,int table, bool player1 = true)
    {
        UnitController controller;

        if (_unusedUnitsPool.Any())
        {
            controller = _unusedUnitsPool.First();
            _blackBoxMap.Add(box, controller);
            MoveTable(controller,table);
        }
        else
        {
            controller = InstantiateUnit(box,table, player1);
            controller.table = table;
        }

        PoolUnit(controller, true);
        return controller;
    }

    [SerializeField] private Table _tablePrefab;

    private List<Table> _tables = new List<Table>();

    private void MoveTable(UnitController unitController, int table)
    {
        if (_tables.Count <= table || _tables[table] == null)
        {
            var t = Instantiate(_tablePrefab);
            t.gameObject.transform.position += (new Vector3(2, 0) * table);
            _tables.Add(t);
        }

        unitController.Move(_tables[table].spawnPoint.transform, Vector3.zero);


    }

    /// <summary>
    /// Creates (or re-uses) a UnitController instance and assigns the Neural Net (IBlackBox) to it and activates it, so that it starts executing the Net.
    /// </summary>
    public void ActivateUnit(IBlackBox box,int table, bool player1 = true)
    {
        UnitController controller = GetUnusedUnit(box, table,player1);
        controller.ActivateUnit(box);
    }

    public override void DeactivateUnit(IBlackBox box)
    {
        if (_blackBoxMap.ContainsKey(box))
        {
            UnitController controller = _blackBoxMap[box];
            controller.DeactivateUnit();

            PoolUnit(controller, false);
        }
    }

    public void RemoveBox(IBlackBox box)
    {
        if (_blackBoxMap.ContainsKey(box))
        {
            _blackBoxMap.Remove(box);
        }
    }
    
}
