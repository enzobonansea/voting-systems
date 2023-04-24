using Election.Interfaces;
using Election.Objects;
using Election.Objects.Exceptions;

namespace Election.Tests;

public class RankedChoiceElectionTest
{
    [Fact]
    public void ItMustHaveBallots()
    {
        Assert.Equal(RankedChoiceElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(null, null)).Message);
        Assert.Equal(RankedChoiceElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(new List<RankedChoiceBallot>(), null)).Message);
    }

    [Fact]
    public void ItMustHaveCandidates()
    {
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(new SimpleVoter(1, "Voter one"), new SimpleCandidate(1, "Candidate one"), 1)
            })
        };
        Assert.Equal(RankedChoiceElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(ballots, null)).Message);
        Assert.Equal(RankedChoiceElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(ballots, new List<ICandidate>())).Message);
    }

    [Fact]
    public void BallotsMustHaveSameVotesQuantity()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo, 2)
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };

        Assert.Throws<BallotsMustHaveSameVotesQuantity>(() => new RankedChoiceElection(ballots, candidates));
    }
}