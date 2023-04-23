using Election.Interfaces;
using Election.Objects;

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
}