using System;
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

    Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome>();
    Dictionary<TGenome, FitnessInfo[]> fitnessDict = new Dictionary<TGenome, FitnessInfo[]>();
    Dictionary<TGenome, int>  interactionDict = new Dictionary<TGenome, int>();
    Dictionary<TGenome, int>  goalsDict = new Dictionary<TGenome, int>();
    Dictionary<TGenome, float>  rotationDict = new Dictionary<TGenome, float>();

    // called by NeatEvolutionAlgorithm at the beginning of a generation
    private IEnumerator EvaluateList(IList<TGenome> genomeList)
    {
        Reset();

        dict = new Dictionary<TGenome, TPhenome>();
        fitnessDict = new Dictionary<TGenome, FitnessInfo[]>();
        rotationDict = new Dictionary<TGenome, float>();
        goalsDict = new Dictionary<TGenome, int>();
        interactionDict = new Dictionary<TGenome, int>();
        
        var organizer = new TournamentOrganizer<TGenome>();

        foreach (var genome in genomeList)
        {
            organizer.registerAgent(genome);
        }

        int rounds = Mathf.CeilToInt(MathF.Log(genomeList.Count,2));
        
        for (int i = 0; i < rounds; i++)
        {
            var matches =  organizer.GetNextMatches();

            yield return ActivateUnits(matches);
        }
        
        
        // evaluate the fitness of all phenomes (IBlackBox) during this trial duration.
        foreach (TGenome genome in dict.Keys)
        {
            TPhenome phenome = dict[genome];

            if (phenome != null)
            {
                _phenomeEvaluator.Evaluate(phenome);
                _neatSupervisor.DeactivateUnit((IBlackBox)phenome);
                FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness(phenome);
                        
                fitnessDict[genome][0] = fitnessInfo;
                interactionDict[genome] = _phenomeEvaluator.GetInteractions(phenome);
                goalsDict[genome] = _phenomeEvaluator.GetGoals(phenome);
                rotationDict[genome] = _phenomeEvaluator.GetRotationTime(phenome);
            }
        }
        
        foreach (TGenome genome in dict.Keys)
        {
            TPhenome phenome = dict[genome];
            if (phenome != null)
            {
                double fitness = 0;

                for (int i = 0; i < _neatSupervisor.Trials; i++)
                {
                    fitness += fitnessDict[genome][i]._fitness;
                }
                var fit = fitness;
                fitness /= _neatSupervisor.Trials; // Averaged fitness
                    
                if (fitness > _neatSupervisor.StoppingFitness)
                {
                    Utility.Log("Fitness is " + fit + ", stopping now because stopping fitness is " + _neatSupervisor.StoppingFitness);
                    //  _phenomeEvaluator.StopConditionSatisfied = true;
                }
                genome.EvaluationInfo.SetFitness(fitness);
                genome.EvaluationInfo.SetInteractions(interactionDict[genome]);
                genome.EvaluationInfo.Goals = goalsDict[genome];
                genome.EvaluationInfo.RotationTime = rotationDict[genome];
                genome.EvaluationInfo.AuxFitnessArr = fitnessDict[genome][0]._auxFitnessArr;
                
                _neatSupervisor.RemoveBox((IBlackBox)phenome);
            }
        }
        yield return 0;
    }

    public void InstantiateUnits(TGenome unitA, int wins, TGenome unitB, int wins2, int table)
    {
        TPhenome phenomeA = _genomeDecoder.Decode(unitA);
        
        if (phenomeA == null)
        {
            // Non-viable genome.
            unitA.EvaluationInfo.SetFitness(0.0);
            unitA.EvaluationInfo.AuxFitnessArr = null;
            return;
        }
        
        _neatSupervisor.ActivateUnit((IBlackBox)phenomeA,table,true);
        _neatSupervisor.AddFitness((IBlackBox)phenomeA,wins);
        var sticks = _neatSupervisor.GetSticks((IBlackBox)phenomeA);

        if (!fitnessDict.ContainsKey(unitA))
        {
            fitnessDict.Add(unitA, new FitnessInfo[1]);
            dict.Add(unitA,phenomeA);
        }

        
        TPhenome phenomeB = _genomeDecoder.Decode(unitB);
        
        if (phenomeB == null)
        {
            // Non-viable genome.
            unitB.EvaluationInfo.SetFitness(0.0);
            unitB.EvaluationInfo.AuxFitnessArr = null;
            return;
        }
        
        _neatSupervisor.ActivateUnit((IBlackBox)phenomeB,table,false);
        _neatSupervisor.AddFitness((IBlackBox)phenomeB,wins2);
        var sticks2 = _neatSupervisor.GetSticks((IBlackBox)phenomeB);
        _neatSupervisor.SetSticks((IBlackBox)phenomeA, sticks2);
        _neatSupervisor.SetSticks((IBlackBox)phenomeB, sticks);
        if (!fitnessDict.ContainsKey(unitB))
        {
            fitnessDict.Add(unitB, new FitnessInfo[1]);
            dict.Add(unitB,phenomeB);
        }
    }
    

    public IEnumerator ActivateUnits(List<(TournamentOrganizer<TGenome>.Competitor, TournamentOrganizer<TGenome>.Competitor)> matches)
    {

        for (int i = 0; i < matches.Count; i++)
        {
            var unitA = matches[i].Item1;
            var unitB = matches[i].Item2;
            InstantiateUnits(unitA.agent,unitA.wins,unitB.agent, unitB.wins,i);
        }
        
        yield return new WaitForSeconds(30);
        
        for (int i = 0; i < matches.Count; i++)
        {
            var phenome =  dict[matches[i].Item1.agent];
            var phenome2 =  dict[matches[i].Item2.agent];
            var box = (IBlackBox)phenome;
            var box2 = (IBlackBox)phenome2;

            int goals1 = _neatSupervisor.GetGoals(box);
            int goals2 = _neatSupervisor.GetGoals(box2);
            _phenomeEvaluator.AddFitness(goals1 >= goals2 ? phenome : phenome2, 1f);
            if (goals1 >= goals2)
            {
                matches[i].Item1.wins++;
            }
            else
            {
                matches[i].Item2.wins++;
            }
        }

        _neatSupervisor.DeactivateAllUnits();
        
        yield return 0;
    }

    public void Reset()
    {
        _phenomeEvaluator.Reset();
    }
}