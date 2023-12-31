// class to handle matchmaking for the single rounds of competition between neural network agents

using System;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Core;
using UnityEngine;

public class TournamentOrganizer<T> where T : class, IGenome<T> {

    // class to keep information about each competitor
    public class Competitor {
        // static ID that counts up
        public static int idCounter = 0;
        // ID for referencing
        public int id;
        // agent object
        public T agent;
        // current score
        public int wins = 0;

        // fitness value (calculated based on tournament result in the end)
        public double fitness = 0.0;

        public Competitor(T playerAgent) {
            agent = playerAgent;
            // give out a unique ID
            id = idCounter;
            // increment idCounter for unique IDs
            idCounter++;
        }
    }

    // list of competitors
    public List<Competitor> competitors = new List<Competitor>();
    private int _round;

    // function to register a new agent as a competitor
    public void registerAgent(T agent) {
        Competitor newComp = new Competitor(agent);
        competitors.Add(newComp);
    }
    
    public List<(Competitor, Competitor)> GetNextMatches()
    {
        _round++;
        // create a dictionary to keep competitors organized in buckets for amount of won games
        IDictionary<int, List<Competitor>> competitorsByWins = new Dictionary<int, List<Competitor>>();
        // add lists for each bucket
        // one more each round, as at least one agent got one win more than before
        for (int bucketNumber = 0; bucketNumber < _round; bucketNumber++) {
            competitorsByWins[bucketNumber] = new List<Competitor>();
        }
        // then, sort all competitors based on their wins so far
        foreach (Competitor comp in competitors) {
            competitorsByWins[comp.wins].Add(comp);
        }
        
        // create matches within buckets
        List<(Competitor, Competitor)> matches = new List<(Competitor, Competitor)>();
        // iterate over buckets
        foreach (List<Competitor> bucket in competitorsByWins.Values)
        {
            if (bucket.Count == 0)
            {
                continue;
            }
            // assure that each bucket has at least 2 competitors and always an even amount of them
            if (bucket.Count % 2 != 0 || bucket.Count < 2)
            {
                throw new Exception("Bucket should have even number of competitors >= 2, but hasn't.");
            }

            // pair first and last element, second and second last, and so on
            // NOTE: this could be randomized but is probably unneccessary
            for (int matchNumber = 0; matchNumber < (bucket.Count / 2); matchNumber++)
            {
                (Competitor, Competitor) newMatch = (bucket[matchNumber], bucket[bucket.Count - matchNumber - 1]);
                matches.Add(newMatch);
            }
        }
        return matches;
    }
}
