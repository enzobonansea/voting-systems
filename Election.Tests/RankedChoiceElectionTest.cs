using Election.Objects;

namespace Election.Tests;

public class RankedChoiceElectionTest
{
    [Fact]
    public void ItMustHaveBallots()
    {
        Assert.Equal(RankedChoiceElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(null, null)).Message);
    }

    [Fact]
    public void ItMustHaveCandidates()
    {
        Assert.Equal(RankedChoiceElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new RankedChoiceElection(new List<RankedChoiceBallot>(), null)).Message);
    }
}