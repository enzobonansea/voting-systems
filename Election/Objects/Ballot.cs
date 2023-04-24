using System;
using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;

namespace Election.Objects
{
    public class SimpleBallot : IBallot<SimpleVote>
    {
        public IEnumerable<SimpleVote> Votes => _votes;
        private List<SimpleVote> _votes;
        
        public SimpleBallot(SimpleVote vote) { _votes = new List<SimpleVote>() { vote }; }
    }

    public class RankedChoiceBallot : IBallot<RankedChoiceVote>
    {
        public IEnumerable<RankedChoiceVote> Votes { get; private set; }
        public RankedChoiceBallot(IEnumerable<RankedChoiceVote> rankedChoiceVotes) { this.Votes = rankedChoiceVotes; }

        public void Remove(ICandidate candidate)
        {
            var candidateVoteRank = this.Votes.First(vote => vote.Candidate == candidate).Rank;
            this.Votes = this.Votes.Where(vote => vote.Candidate != candidate);
            foreach (var vote in Votes.Where(vote => vote.Rank > candidateVoteRank))
            {
                vote.Rank--;
            }
        }

        public bool Has(ICandidate candidate)
        {
            return this.Votes.Any(vote => vote.Candidate == candidate);
        }    
    }
}
