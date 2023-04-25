using System;
using System.Collections.Generic;
using System.Linq;
using Election.Extensions;
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
            var candidatesIds = this.Candidates.Select(candidate => candidate.Id).ToHashSet();
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
            if (Ballots.SelectMany(ballot => ballot.Votes).Select(vote => vote.Voter.Id).HasDuplicates())
            {
                throw new PeopleCannotVoteMoreThanOnce();
            }
        }

        public override void CountVotes()
        {
            var votesPerCandidate =  this.Candidates.ToDictionary(candidate => candidate.Id, _ => 0);

            foreach (var ballot in Ballots.SelectMany(ballot => ballot.Votes))
            {
                votesPerCandidate[ballot.Candidate.Id] += 1;
            }

            var maximumVotes = -1;
            var winnerId = -1;
            foreach (var votesOfCandidate in votesPerCandidate)
            {
                if (votesOfCandidate.Value > maximumVotes)
                {
                    maximumVotes = votesOfCandidate.Value;
                    winnerId = votesOfCandidate.Key;
                }
            }
            
            this.Winner = this.Candidates.First(candidate => candidate.Id == winnerId);
        }
    }

    public class RankedChoiceElection : Election<RankedChoiceBallot, RankedChoiceVote>
    {
        private RankedChoiceElectionRound lastRound;
        
        public RankedChoiceElection(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates)
            : base(ballots, candidates)
        {
            this.EnsureBallotsSameVotesQuantity();
            this.EnsureBallotsVotesHaveDifferentRanks();
            this.EnsureRanksBetweenOneAndVotesLength();
            this.EnsureSameVoterInBallots();
            this.EnsureDifferentCandidatesInBallots();
        }
        
        public int GetFirstPreferenceVotes(ICandidate candidate)
        {
            return this.lastRound.GetFirstPreferenceVotes(candidate);
        }

        private void EnsureDifferentCandidatesInBallots()
        {
            foreach (var ballot in Ballots)
            {
                if (ballot.Votes.Select(vote => vote.Candidate.Id).HasDuplicates())
                {
                    throw new BallotsMustHaveDifferentCandidates();
                }
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
                if (ballot.Votes.Select(vote => vote.Rank).HasDuplicates())
                {
                    throw new BallotsVotesMustHaveDifferentRanks();
                }
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
            if (Ballots.Select(ballot => ballot.Votes.First().Voter.Id).HasDuplicates())
            {
                throw new PeopleCannotVoteMoreThanOnce();
            }
        }

        public override void CountVotes()
        {
            this.lastRound = new RankedChoiceElectionRound(Ballots, Candidates);
            do this.lastRound.Next(); while (!this.lastRound.WonByAbsoluteMajority);
            this.Candidates = this.lastRound.Candidates;
            this.Winner = this.lastRound.Winner;
        }
    }
}
