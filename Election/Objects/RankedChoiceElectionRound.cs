using System;
using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;

namespace Election.Objects
{
    public class RankedChoiceElectionRound
    {
        private Dictionary<ICandidate, int> firstPreferenceVotesPerCandidate;
        private ICandidate winner;
        private ICandidate loser;

        public RankedChoiceElectionRound(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            var firstPreferenceVotes = ballots.SelectMany(ballot => ballot.Votes).Where(vote => vote.Rank == 1);
            this.firstPreferenceVotesPerCandidate = candidates.ToDictionary(candidate => candidate, _ => 0);
            foreach (var vote in firstPreferenceVotes) this.firstPreferenceVotesPerCandidate[vote.Candidate] += 1;

            var firstPreferenceVotesPerCandidateOrdered = this.firstPreferenceVotesPerCandidate
                .OrderByDescending(votesPerCandidate => votesPerCandidate.Value)
                .ToList();

            this.winner = firstPreferenceVotesPerCandidateOrdered.First().Key;
            this.loser = firstPreferenceVotesPerCandidateOrdered.Last().Key;
        }

        public ICandidate Winner
        {
            get => this.winner;
        }

        public ICandidate Loser
        {
            get => this.loser;
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