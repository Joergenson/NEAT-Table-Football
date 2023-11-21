// class to handle matchmaking for the single rounds of competition between neural network agents
public class TournamentOrganizer<T> {

    // class to keep information about each competitor
    class Competitor<T> {
        // static ID that counts up
        public static int idCounter = 0;
        // ID for referencing
        public id;
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
    public List<Competitor<T>> competitors = new List<Competitor<T>>();

    // function to register a new agent as a competitor
    public void registerAgent(T agent) {
        Competitor newComp = new Competitor(agent);
        competitors.Add(newComp);
    }

    // function to simulate a complete tournament and assign fitness values to agents based on their placements
    public void simulateFullTournament(int rounds) {
        // only works for populations of size 2^x
        popSize = competitors.Count;
        if  (popSize < 2 || (popSize & (popSize - 1) != 0)) {
            throw new Exception("Population size needs to be a power of 2 and at least 2, but is not.");
        }

        // perform one iteration for each round that should be played
        for (int round = 0; round < rounds; round++) {
            // create a dictionary to keep competitors organized in buckets for amount of won games
            IDictionary<int, List<Competitor<T>>> competitorsByWins = new Dictionary<int, List<Competitor<T>>>();
            // add lists for each bucket
            // one more each round, as at least one agent got one win more than before
            for (int bucketNumber = 0; bucketNumber <= round; bucketNumber++) {
                competitorsByWins[bucketNumber] = new List<Competitor<T>>();
            }
            // then, sort all competitors based on their wins so far
            foreach (Competitor<T> comp in competitors) {
                competitorsByWins[comp.wins].Add(comp);
            }
            
            // create matches within buckets
            List<(Competitor<T>, Competitor<T>)> matches = new List<(Competitor<T>, Competitor<T>)>();
            // iterate over buckets
            foreach(List<Competitor<T>> bucket in competitorsByWins.Values) {
                // assure that each bucket has at least 2 competitors and always an even amount of them
                if (bucket.Count % 2 != 0 || bucket.Count < 2) {
                    throw new Exception("Bucket should have even number of competitors >= 2, but hasn't.");
                }

                // pair first and last element, second and second last, and so on
                // NOTE: this could be randomized but is probably unneccessary
                for (int matchNumber = 0; matchNumber < (bucket.Count / 2)) {
                    (Competitor<T>, Competitor<T>) newMatch = (bucket[matchNumber], bucket[bucket.Count - matchNumber]);
                    matches.Add(newMatch);
                }
            }

            foreach ((Competitor<T>, Competitor<T>) match in matches) {
                // simulate the match --- This is very subject to change, but should somehow happen
                // and then return a boolean if the first agent won the match
                compOneWon = simulateMatch(match);
                // adjust score for winner
                if (compOneWon) {
                    match.Item1.wins++;
                } else {
                    match.Item2.wins++;
                }
            }
        }
        
        // after last round, assign fitness to each competitor (wins - losses)
        // NOTE: might be necessary to scale the fitness value to an interval?
        foreach (Competitor<T> comp in competitors) {
            comp.fitness = 2.0 * comp.wins - rounds;
        }
    }
}