using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;

namespace Election.Objects
{
    public class RankedChoiceElectionRound
    {
        private readonly Dictionary<ICandidate, int> firstPreferenceVotesPerCandidate;
        private readonly ICandidate winner;
        private readonly ICandidate loser;
        private readonly bool wonByAbsoluteMajority;

        public RankedChoiceElectionRound(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            var firstPreferenceVotes = ballots.SelectMany(ballot => ballot.Votes).Where(vote => vote.Rank == 1).ToList();
            this.firstPreferenceVotesPerCandidate = candidates.ToDictionary(candidate => candidate, _ => 0);
            foreach (var vote in firstPreferenceVotes) this.firstPreferenceVotesPerCandidate[vote.Candidate] += 1;

            var firstPreferenceVotesPerCandidateOrdered = this.firstPreferenceVotesPerCandidate
                .OrderByDescending(votesPerCandidate => votesPerCandidate.Value)
                .ToList();

            this.winner = firstPreferenceVotesPerCandidateOrdered.First().Key;
            this.loser = firstPreferenceVotesPerCandidateOrdered.Last().Key;
            this.wonByAbsoluteMajority = (firstPreferenceVotesPerCandidateOrdered.First().Value / (decimal)firstPreferenceVotes.Count()) > (decimal)0.5;
        }

        public ICandidate Winner
        {
            get => this.winner;
        }

        public ICandidate Loser
        {
            get => this.loser;
        }

        public bool WonByAbsoluteMajority
        {
            get => wonByAbsoluteMajority;
        }

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
    }
}