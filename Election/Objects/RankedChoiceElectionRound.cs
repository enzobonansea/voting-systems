using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;
using Election.Objects.Exceptions;

namespace Election.Objects
{
    public class RankedChoiceElectionRound
    {
        private Dictionary<ICandidate, int> firstPreferenceVotesPerCandidate;

        public RankedChoiceElectionRound(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            this.Ballots = ballots;
            this.Candidates = candidates;
            CalculateWinnerAndLoser();
        }

        /// <summary>
        /// Calculates winner and loser from ballots
        /// Running time complexity: O(B*V + C) where B = quantity of ballots, C = quantity of candidates,
        /// V = quantity of votes per ballot
        /// </summary>
        private void CalculateWinnerAndLoser()
        {
            var firstPreferenceVotes = Ballots
                .SelectMany(ballot => ballot.Votes)
                .Where(vote => vote.Rank == 1)
                .ToList(); // O(B * V)
            this.firstPreferenceVotesPerCandidate = Candidates.ToDictionary(candidate => candidate, _ => 0); // O(C)
            foreach (var vote in firstPreferenceVotes) // O(B * V)
            {
                this.firstPreferenceVotesPerCandidate[vote.Candidate] += 1; // O(1)
            }
        
            KeyValuePair<ICandidate, int> winner = new KeyValuePair<ICandidate, int>(null, int.MinValue); // O(1)
            KeyValuePair<ICandidate, int> loser = new KeyValuePair<ICandidate, int>(null, int.MaxValue);  // O(1)
            foreach (var voteOfCandidate in firstPreferenceVotesPerCandidate) // O(C)
            {
                if (voteOfCandidate.Value >= winner.Value) winner = voteOfCandidate; // O(1)
                if (voteOfCandidate.Value <= loser.Value) loser = voteOfCandidate;   // O(1)
            }
        
            this.Winner = winner.Key; // O(1)
            this.Loser = loser.Key; // O(1)
            this.WonByAbsoluteMajority = (winner.Value / (decimal)firstPreferenceVotes.Count()) > (decimal)0.5; // O(1)
        }

        public ICandidate Winner { get; private set; }
        public ICandidate Loser { get; private set; }
        public bool WonByAbsoluteMajority { get; private set; }
        public IEnumerable<RankedChoiceBallot> Ballots { get; private set; }
        public IEnumerable<ICandidate> Candidates { get; private set; }

        public int GetFirstPreferenceVotes(ICandidate candidate)
        {
            if (this.firstPreferenceVotesPerCandidate.TryGetValue(candidate, out var votes))
            {
                return votes;
            }
            else
            {
                throw new CandidateHasNotFirstPreferenceVotes();
            }
        }

        /// <summary>
        /// Calculates next round.
        /// Running time complexity: O(B*V + C) where B = quantity of ballots, C = quantity of candidates,
        /// V = quantity of votes per ballot
        /// </summary>
        public void Next()
        {
            if (this.WonByAbsoluteMajority) return; // O(1)

            this.Candidates = this.Candidates.Where(candidate => candidate != this.Loser).ToList(); // O(C)
            foreach (var ballot in Ballots) // O(B * V)
            {
                if (ballot.Has(this.Loser)) // O(V)
                {
                    ballot.Remove(this.Loser); // O(V)
                }
            }

            this.CalculateWinnerAndLoser(); // O(B*V + C)
        }
    }
}