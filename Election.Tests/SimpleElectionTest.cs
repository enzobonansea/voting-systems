using Election.Interfaces;
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

    [Fact]
    public void ItMustWinWhoseHaveMoreVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");

        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate three");
        
        var ballots = new List<SimpleBallot>
        {
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
            new SimpleBallot(new SimpleVote(voterTwo, candidateOne)),
            new SimpleBallot(new SimpleVote(voterThree, candidateTwo)),
        };

        var election = new SimpleElection(ballots, new List<ICandidate>() { candidateOne, candidateTwo });
        
        election.CountVotes();
        
        Assert.Equal(candidateOne, election.Winner);
    }
}