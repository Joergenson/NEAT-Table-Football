using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Phenomes;
using UnityEngine;
using UnityEngine.Serialization;
using UnitySharpNEAT;

public class PlayerController : UnitController
{
    [SerializeField] private Stick[] sticks;
    [SerializeField] private Stick[] _enemySticks;
    [SerializeField] public Ball ball;
    [SerializeField] private Goal _goal, _ownGoal;
    private float _fitness;

    protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
    {
        inputSignalArray[0] = sticks[0].GetStickPosition();
        inputSignalArray[1] = sticks[0].GetStickVelocity();
        inputSignalArray[2] = sticks[0].GetStickAngularVelocity();
        
        inputSignalArray[3] = sticks[1].GetStickPosition();
        inputSignalArray[4] = sticks[1].GetStickVelocity();
        inputSignalArray[5] = sticks[1].GetStickAngularVelocity();
        
        inputSignalArray[6] = sticks[2].GetStickPosition();
        inputSignalArray[7] = sticks[2].GetStickVelocity();
        inputSignalArray[8] = sticks[2].GetStickAngularVelocity();
        
        inputSignalArray[9] = sticks[3].GetStickPosition();
        inputSignalArray[10] = sticks[3].GetStickVelocity();
        inputSignalArray[11] = sticks[3].GetStickAngularVelocity();

        if (_enemySticks != null)
        {
            inputSignalArray[12] = _enemySticks[0].GetStickPosition();
            inputSignalArray[13] = _enemySticks[0].GetStickVelocity();
            inputSignalArray[14] = _enemySticks[0].GetStickAngularVelocity();
            
            inputSignalArray[15] = _enemySticks[1].GetStickPosition();
            inputSignalArray[16] = _enemySticks[1].GetStickVelocity();
            inputSignalArray[17] = _enemySticks[1].GetStickAngularVelocity();
            
            inputSignalArray[18] = _enemySticks[2].GetStickPosition();
            inputSignalArray[19] = _enemySticks[2].GetStickVelocity();
            inputSignalArray[20] = _enemySticks[2].GetStickAngularVelocity();
            
            inputSignalArray[21] = _enemySticks[3].GetStickPosition();
            inputSignalArray[22] = _enemySticks[3].GetStickVelocity();
            inputSignalArray[23] = _enemySticks[3].GetStickAngularVelocity();
        }

        var ballPos = ball.GetBallPosition();
        inputSignalArray[24] = ballPos.x;
        inputSignalArray[25] = ballPos.y;
        inputSignalArray[26] = ballPos.z;

        var ballVel = ball.GetBallVelocity();
        inputSignalArray[27] = ballVel.x;
        inputSignalArray[28] = ballVel.y;
        inputSignalArray[29] = ballVel.z;
    }

    protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
    {
        sticks[0].MoveStick((float)outputSignalArray[0]);
        sticks[0].RotateStick((float)outputSignalArray[1]);
        sticks[1].MoveStick((float)outputSignalArray[2]);
        sticks[1].RotateStick((float)outputSignalArray[3]);
        sticks[2].MoveStick((float)outputSignalArray[4]);
        sticks[2].RotateStick((float)outputSignalArray[5]);
        sticks[3].MoveStick((float)outputSignalArray[6]);
        sticks[3].RotateStick((float)outputSignalArray[7]);
    }

    public override float GetFitness()
    {
        // var overallInt = sticks.Sum(t => t.GetInteractions());
        // var interaction_fit = Mathf.Min(overallInt / 10f,1f);
        //
        // var g = _goal.GetGoals();
        // var goal_fit = Mathf.Min(g / 3f, 1f);
        //
        // var goals_missed = _ownGoal.GetGoals();
        // var goal_penalty =  Mathf.Min(goals_missed,3);
        //
        // var rotationTimes = sticks.Average(t => t.rotationTimer);
        // var rotationPenalty = Mathf.Clamp(0.1f * rotationTimes, 0f, 10f);
        //
        // var fitness = (0.3f * interaction_fit + 0.7f * goal_fit - rotationPenalty - goal_penalty);
        // return Mathf.Max(0f,fitness);
        return _fitness;
    }

    protected override void HandleIsActiveChanged(bool newIsActive)
    {
        if (newIsActive)
        {
            _fitness = 0;
            foreach (var stick in sticks)
            {
                stick.rotationTimer = 0;
                stick.interactions = 0;
                _goal.SetGoals(0);
            }
        }
        
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(newIsActive);
        }
    }

    public override void Move(Transform parent, Vector3 position)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;

        foreach (Stick stick in sticks)
        {
            stick.joint.autoConfigureConnectedAnchor = false;
            var pos = stick.transform.localPosition;
            pos.x = 0;
            stick.transform.localPosition = pos;
            stick.joint.autoConfigureConnectedAnchor = true;
        }
    }

    public override int GetGoals()
    {
        return _goal.GetGoals();
    }

    public override void AddFitness(float fit)
    {
        _fitness += fit;
    }

    public override int GetInteractions()
    {
        return sticks.Sum(t => t.GetInteractions());
    }

    public override float GetRotationTime()
    {
        return sticks.Sum(t => t.rotationTimer);
    }

    public override Stick[] GetSticks()
    {
        return sticks;
    }

    public override void SetEnemySticks(Stick[] enemySticks)
    {
        this._enemySticks = enemySticks;
    }
}
