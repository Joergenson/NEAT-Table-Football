using System.Collections;
using System.Collections.Generic;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;


public class CustomCoroutinedListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
    where TGenome : class, IGenome<TGenome>
    where TPhenome : class
{
    [SerializeField] private readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;

    [SerializeField] private IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

    [SerializeField] private TurnamnetNEATSupervisor _neatSupervisor;

    #region Constructor

    /// <summary>
    /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
    /// </summary>
    public CustomCoroutinedListEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
        IPhenomeEvaluator<TPhenome> phenomeEvaluator,
        TurnamnetNEATSupervisor neatSupervisor)
    {
        _genomeDecoder = genomeDecoder;
        _phenomeEvaluator = phenomeEvaluator;
        _neatSupervisor = neatSupervisor;
    }

    #endregion

    public ulong EvaluationCount
    {
        get { return _phenomeEvaluator.EvaluationCount; }
    }

    public bool StopConditionSatisfied
    {
        get { return _phenomeEvaluator.StopConditionSatisfied; }
    }

    public IEnumerator Evaluate(IList<TGenome> genomeList)
    {
        yield return EvaluateList(genomeList);
    }

// called by NeatEvolutionAlgorithm at the beginning of a generation
    private IEnumerator EvaluateList(IList<TGenome> genomeList)
    {
        Reset();

        for (int i = 0; i < genomeList.Count; i++)
        {
            TGenome genomeA = genomeList[i];
            TPhenome phenomeA = _genomeDecoder.Decode(genomeA);

            if (phenomeA == null)
            {
                // Non-viable genome.
                genomeA.EvaluationInfo.SetFitness(0.0);
                genomeA.EvaluationInfo.AuxFitnessArr = null;
                continue;
            }
            
            _neatSupervisor.ActivateUnit((IBlackBox)phenomeA,true);

            yield return new WaitForSeconds(5);
            
            _neatSupervisor.DeactivateUnit((IBlackBox)phenomeA);
        }

        yield return 0;
    }

    public void Reset()
    {
        _phenomeEvaluator.Reset();
    }
}