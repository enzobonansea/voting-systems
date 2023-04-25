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
            var candidates = this.Candidates.ToHashSet();
            var someCandidateIsInvalid = this.Ballots
                .SelectMany(ballot => ballot.Votes)
                .Select(vote => vote.Candidate)
                .Any(candidate => !candidates.Contains(candidate));

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
            if (Ballots.SelectMany(ballot => ballot.Votes).Select(vote => vote.Voter).HasDuplicates())
            {
                throw new PeopleCannotVoteMoreThanOnce();
            }
        }
        
        /// <summary>
        /// Count all votes
        /// Running time complexity: O(C + B*V) where C = quantity of candidates, B = quantity of ballots,
        /// V = quantity of votes per ballot
        /// </summary>
        public override void CountVotes()
        {
            var votesPerCandidate =  this.Candidates.ToDictionary(candidate => candidate, _ => 0); // O(C)

            foreach (var ballot in Ballots.SelectMany(ballot => ballot.Votes)) // O(B * V)
            {
                votesPerCandidate[ballot.Candidate] += 1; // O(1)
            }

            var maximumVotes = int.MinValue; // O(1)
            ICandidate winner = null; // O(1)
            foreach (var votesOfCandidate in votesPerCandidate) // O(C)
            {
                if (votesOfCandidate.Value > maximumVotes) // O(1)
                {
                    maximumVotes = votesOfCandidate.Value; // O(1)
                    winner = votesOfCandidate.Key; // O(1)
                }
            }
            
            this.Winner = this.Candidates.First(candidate => candidate == winner); // O(C)
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
                if (ballot.Votes.Select(vote => vote.Candidate).HasDuplicates())
                {
                    throw new BallotsMustHaveDifferentCandidates();
                }
            }
        }

        private void EnsureSameVoterInBallots()
        {
            foreach (var ballot in Ballots)
            {
                var voter = ballot.Votes.First().Voter;
                if (ballot.Votes.Skip(1).Select(vote => vote.Voter).Any(otherVoter => otherVoter != voter))
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
            if (Ballots.Select(ballot => ballot.Votes.First().Voter).HasDuplicates())
            {
                throw new PeopleCannotVoteMoreThanOnce();
            }
        }

        /// <summary>
        /// Count all votes
        /// Running time complexity: O(R*(C + B*V)) where C = quantity of candidates, B = quantity of ballots,
        /// V = quantity of votes per ballot and R = quantity of rounds. R is in O(C), then time complexity is
        /// O(C² + C*B*V)
        /// </summary>
        public override void CountVotes()
        {
            var round = new RankedChoiceElectionRound(Ballots, Candidates);
            do round.Next(); while (!round.WonByAbsoluteMajority);
            this.Candidates = round.Candidates;
            this.Winner = round.Winner;
        }
    }
}
