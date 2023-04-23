using Election.Objects;

namespace Election.Tests;

public class SimpleElectionTest
{
    [Fact]
    public void ItMustHaveBallots()
    {
        Assert.Equal(SimpleElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(null, null)).Message);
    }

    [Fact]
    public void ItMustHaveCandidates()
    {
        Assert.Equal(SimpleElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(new List<SimpleBallot>(), null)).Message);
    }
}