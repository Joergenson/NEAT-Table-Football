/*
------------------------------------------------------------------
  This file is part of UnitySharpNEAT 
  Copyright 2020, Florian Wolf
  https://github.com/flo-wolf/UnitySharpNEAT
------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using System;

namespace UnitySharpNEAT
{
    /// <summary>
    /// This class is an Implementation of a PhenomeEvaluator, 
    /// which evaluates the performance (fitness) of the IBlackBox's (Neural Nets) of our instantiated Units.
    /// </summary>
    [Serializable]
    public class BlackBoxFitnessEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        [SerializeField]
        private ulong _evalCount;

        [SerializeField]
        private bool _stopConditionSatisfied;

        [SerializeField]
        private NeatSupervisor _neatSupervisor;

        private Dictionary<IBlackBox, FitnessInfo> _fitnessByBox = new Dictionary<IBlackBox, FitnessInfo>();
        private Dictionary<IBlackBox, int> _interactionsByBox = new Dictionary<IBlackBox, int>();

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public BlackBoxFitnessEvaluator(NeatSupervisor neatSupervisor)
        {
            this._neatSupervisor = neatSupervisor;
        }

        public void Evaluate(IBlackBox box)
        {
            if (_neatSupervisor != null)
            {
                float fit = _neatSupervisor.GetFitness(box);
                int inter = _neatSupervisor.GetInteractions(box);
                _interactionsByBox.Add(box, inter);

                FitnessInfo fitness = new FitnessInfo(fit, fit);
                _fitnessByBox.Add(box, fitness);
            }
        }

        public void AddFitness(IBlackBox box, float fit)
        {
            if (_neatSupervisor != null)
            {
                _neatSupervisor.AddFitness(box, fit);
            }
        }

        public void Reset()
        {
            _fitnessByBox = new Dictionary<IBlackBox, FitnessInfo>();
        }

        public int GetInteractions(IBlackBox phenome)
        {
            if (_interactionsByBox.ContainsKey(phenome))
            {
                int inter = _interactionsByBox[phenome];
                _interactionsByBox.Remove(phenome);
                return inter;
            }

            return 0;
        }

        public FitnessInfo GetLastFitness(IBlackBox phenome)
        {
            if (_fitnessByBox.ContainsKey(phenome))
            {
                FitnessInfo fit = _fitnessByBox[phenome];
                _fitnessByBox.Remove(phenome);

                return fit;
            }

            return FitnessInfo.Zero;
        }
    }
}
