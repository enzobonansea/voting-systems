using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;
using Election.Objects.Exceptions;

namespace Election.Objects
{
    public class RankedChoiceElectionRound
    {
        private Dictionary<ICandidate, int> firstPreferenceVotesPerCandidate;
        private ICandidate winner;
        private ICandidate loser;
        private bool wonByAbsoluteMajority;

        public RankedChoiceElectionRound(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            this.Ballots = ballots;
            this.Candidates = candidates;
            CalculateWinnerAndLoser();
        }

        private void CalculateWinnerAndLoser()
        {
            var firstPreferenceVotes = Ballots.SelectMany(ballot => ballot.Votes).Where(vote => vote.Rank == 1).ToList();
            this.firstPreferenceVotesPerCandidate = Candidates.ToDictionary(candidate => candidate, _ => 0);
            foreach (var vote in firstPreferenceVotes) this.firstPreferenceVotesPerCandidate[vote.Candidate] += 1;

            var firstPreferenceVotesPerCandidateOrdered = this.firstPreferenceVotesPerCandidate
                .OrderByDescending(votesPerCandidate => votesPerCandidate.Value)
                .ToList();

            this.winner = firstPreferenceVotesPerCandidateOrdered.First().Key;
            this.loser = firstPreferenceVotesPerCandidateOrdered.Last().Key;
            this.wonByAbsoluteMajority =
                (firstPreferenceVotesPerCandidateOrdered.First().Value / (decimal)firstPreferenceVotes.Count()) > (decimal)0.5;
        }

        public ICandidate Winner { get => this.winner; }
        public ICandidate Loser { get => this.loser; }
        public bool WonByAbsoluteMajority { get => wonByAbsoluteMajority; }
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

        public void Next()
        {
            if (this.WonByAbsoluteMajority) return;

            this.Candidates = this.Candidates.Where(candidate => candidate != this.Loser).ToList();
            foreach (var ballot in Ballots)
            {
                if (ballot.Has(this.Loser))
                {
                    ballot.Remove(this.Loser);
                }
            }
            
            this.CalculateWinnerAndLoser();
        }
    }
}