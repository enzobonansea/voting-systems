using System;
using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;
using Election.Objects.Exceptions;

namespace Election.Objects
{
    public abstract class Election<TBallot, TVote> : IElection<TBallot, TVote> where TBallot : IBallot<TVote> where TVote : IVote
    {
        public IEnumerable<TBallot> Ballots { get; protected set; }
        public IEnumerable<ICandidate> Candidates { get; protected set; }
        public ICandidate Winner { get; protected set; }

        public Election(IEnumerable<TBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            this.Ballots = ballots;
            this.Candidates = candidates;
        }

        public abstract void CountVotes();
    }

    public class SimpleElection : Election<SimpleBallot, SimpleVote>
    {
        public const string ItMustHaveBallotsErrorMessage = "Simple election must have ballots";
        public const string ItMustHaveCandidatesErrorMessage = "Simple election must have candidates";

        public SimpleElection(IEnumerable<SimpleBallot> ballots, IEnumerable<ICandidate> candidates)
            : base(ballots, candidates)
        {
            if (ballots is null) throw new ArgumentException(ItMustHaveBallotsErrorMessage);
            if (candidates is null) throw new ArgumentException(ItMustHaveCandidatesErrorMessage);
            this.EnsureOneVotePerPerson();
        }

        private void EnsureOneVotePerPerson()
        {
            var votersListed = this.Ballots.SelectMany(ballot => ballot.Votes).Select(vote => vote.Voter.Id).ToList();
            var nonRepeatedVoters = new HashSet<int>(votersListed);
            if (nonRepeatedVoters.Count < votersListed.Count) throw new PeopleCannotVoteMoreThanOnce();
        }

        public override void CountVotes()
        {
            var votesPerCandidate =  this.Candidates.ToDictionary(candidate => candidate.Id, _ => 0);

            foreach (var ballot in Ballots.SelectMany(ballot => ballot.Votes))
            {
                votesPerCandidate[ballot.Candidate.Id] += 1;
            }
            
            var winnerId = votesPerCandidate.OrderByDescending(votes => votes.Value).First().Key;

            this.Winner = this.Candidates.First(candidate => candidate.Id == winnerId);
        }
    }

    class RankedChoiceElection : Election<RankedChoiceBallot, RankedChoiceVote>
    {
        public RankedChoiceElection(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates) : base(ballots, candidates) { }

        public override void CountVotes()
        {
            throw new NotImplementedException();
        }
    }
}
