using System;
using System.Collections.Generic;

using Election.Interfaces;

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
        }

        public override void CountVotes()
        {
            throw new NotImplementedException();
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
