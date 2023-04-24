using System;
using System.Collections.Generic;
using System.Linq;
using Election.Interfaces;
using Election.Objects.Exceptions;

namespace Election.Objects
{
    public abstract class Election<TBallot, TVote> : IElection<TBallot, TVote> where TBallot : IBallot<TVote> where TVote : IVote
    {
        public const string ItMustHaveBallotsErrorMessage = "Simple election must have ballots";
        public const string ItMustHaveCandidatesErrorMessage = "Simple election must have candidates";
        
        public IEnumerable<TBallot> Ballots { get; protected set; }
        public IEnumerable<ICandidate> Candidates { get; protected set; }
        public ICandidate Winner { get; protected set; }

        public Election(IEnumerable<TBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            if (ballots is null || !ballots.Any()) throw new ArgumentException(ItMustHaveBallotsErrorMessage);
            if (candidates is null || !candidates.Any()) throw new ArgumentException(ItMustHaveCandidatesErrorMessage);
            
            this.Ballots = ballots;
            this.Candidates = candidates;
            
            this.EnsureOneVotePerPerson();
            this.EnsureCandidatesAreValid();
        }
        
        private void EnsureCandidatesAreValid()
        {
            var candidatesIds = this.Candidates.Select(candidate => candidate.Id);
            var someCandidateIsInvalid = this.Ballots
                .SelectMany(ballot => ballot.Votes)
                .Select(vote => vote.Candidate.Id)
                .Any(candidateId => !candidatesIds.Contains(candidateId));

            if (someCandidateIsInvalid) throw new InvalidCandidate();
        }

        protected abstract void EnsureOneVotePerPerson();

        public abstract void CountVotes();
    }

    public class SimpleElection : Election<SimpleBallot, SimpleVote>
    {
        public SimpleElection(IEnumerable<SimpleBallot> ballots, IEnumerable<ICandidate> candidates)
            : base(ballots, candidates)
        {
        }

        protected override void EnsureOneVotePerPerson()
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

    public class RankedChoiceElection : Election<RankedChoiceBallot, RankedChoiceVote>
    {
        public RankedChoiceElection(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
            : base(ballots, candidates)
        {
            this.EnsureBallotsSameVotesQuantity();
            this.EnsureBallotsVotesHaveDifferentRanks();
            this.EnsureRanksBetweenOneAndVotesLength();
            this.EnsureSameVoterInBallots();
            this.EnsureDifferentCandidatesInBallots();
        }

        private void EnsureDifferentCandidatesInBallots()
        {
            foreach (var ballot in Ballots)
            {
                var candidatesList = ballot.Votes.Select(vote => vote.Candidate.Id).ToList();
                var candidatesSet = new HashSet<int>(candidatesList);
                if (candidatesSet.Count < candidatesList.Count) throw new BallotsMustHaveDifferentCandidates();
            }
        }

        private void EnsureSameVoterInBallots()
        {
            foreach (var ballot in Ballots)
            {
                var voterId = ballot.Votes.First().Voter.Id;
                if (ballot.Votes.Skip(1).Select(vote => vote.Voter.Id).Any(otherVoterId => otherVoterId != voterId))
                {
                    throw new BallotsMustHaveSameVoter();
                }
            }
        }

        private void EnsureRanksBetweenOneAndVotesLength()
        {
            var votesLength = Ballots.First().Votes.Count();
            if (Ballots.SelectMany(ballot => ballot.Votes).Any(vote => vote.Rank < 1 || vote.Rank > votesLength))
            {
                throw new BallotsMustHaveRanksBetweenOneAndVotesLength();
            }
        }

        private void EnsureBallotsVotesHaveDifferentRanks()
        {
            foreach (var ballot in Ballots)
            {
                var ranksList = ballot.Votes.Select(vote => vote.Rank).ToList();
                var ranksSet = new HashSet<int>(ranksList);
                if (ranksSet.Count < ranksList.Count) throw new BallotsVotesMustHaveDifferentRanks();
            }
        }

        private void EnsureBallotsSameVotesQuantity()
        {
            var representative = Ballots.First();
            if (Ballots.Skip(1).Any(ballot => ballot.Votes.Count() != representative.Votes.Count()))
            {
                throw new BallotsMustHaveSameVotesQuantity();
            }
        }

        protected override void EnsureOneVotePerPerson()
        {
            var votersList = Ballots.Select(ballot => ballot.Votes.First().Voter.Id).ToList();
            var votersSet = new HashSet<int>(votersList);
            if (votersSet.Count < votersList.Count) throw new PeopleCannotVoteMoreThanOnce();
        }

        public override void CountVotes()
        {
            throw new NotImplementedException();
        }
    }
}
