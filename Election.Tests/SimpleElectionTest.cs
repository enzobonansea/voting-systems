using Election.Interfaces;
using Election.Objects;
using Election.Objects.Exceptions;

namespace Election.Tests;

public class SimpleElectionTest
{
    [Fact]
    public void ItMustHaveBallots()
    {
        Assert.Equal(SimpleElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(null, null)).Message);
        Assert.Equal(SimpleElection.ItMustHaveBallotsErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(new List<SimpleBallot>(), null)).Message);
    }

    [Fact]
    public void ItMustHaveCandidates()
    {
        var ballots = new List<SimpleBallot>
        {
            new SimpleBallot(new SimpleVote(new SimpleVoter(1, "Voter one"), new SimpleCandidate(1, "Candidate one")))
        };
        Assert.Equal(SimpleElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(ballots, null)).Message);
        Assert.Equal(SimpleElection.ItMustHaveCandidatesErrorMessage, Assert.Throws<ArgumentException>(() => new SimpleElection(ballots, new List<ICandidate>())).Message);
    }

    [Fact]
    public void PeopleCannotVoteMoreThanOnce()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var invalidBallots = new List<SimpleBallot>
        {
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
        };
        var candidates = new List<ICandidate>() { candidateOne };
        
        Assert.Throws<PeopleCannotVoteMoreThanOnce>(() => new SimpleElection(invalidBallots, candidates));
    }

    [Fact]
    public void VotedCandidatesMustBeValid()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var invalidBallots = new List<SimpleBallot>
        {
            new SimpleBallot(new SimpleVote(voterOne, new SimpleCandidate(2, "Candidate two"))),
        };
        var candidates = new List<ICandidate>() { candidateOne };
        
        Assert.Throws<InvalidCandidate>(() => new SimpleElection(invalidBallots, candidates));
    }
    
    [Fact]
    public void ItMustWinWhoseHaveAbsoluteMajorityOfVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");

        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        
        var ballots = new List<SimpleBallot>
        {
            // Candidate one: 66.66%
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
            new SimpleBallot(new SimpleVote(voterTwo, candidateOne)),
            // Candidate two: 33.33%
            new SimpleBallot(new SimpleVote(voterThree, candidateTwo)),
        };

        var election = new SimpleElection(ballots, new List<ICandidate>() { candidateOne, candidateTwo });
        
        election.CountVotes();
        
        Assert.Equal(candidateOne, election.Winner);
    }
    
    [Fact]
    public void ItMustWinWhoseHaveMoreVotesButNotAbsoluteMajorityOfVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var voterFour = new SimpleVoter(4, "Voter four");

        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateThree = new SimpleCandidate(3, "Candidate three");
        
        var ballots = new List<SimpleBallot>
        {
            // Candidate one: 50%
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
            new SimpleBallot(new SimpleVote(voterTwo, candidateOne)),
            // Candidate two: 25%
            new SimpleBallot(new SimpleVote(voterThree, candidateTwo)),
            // Candidate three: 25%
            new SimpleBallot(new SimpleVote(voterFour, candidateThree)),
        };

        var election = new SimpleElection(ballots, new List<ICandidate>() { candidateOne, candidateTwo, candidateThree });
        
        election.CountVotes();
        
        Assert.Equal(candidateOne, election.Winner);
    }
    
    [Fact]
    public void ItMustThrowExceptionOnTie()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        
        var ballots = new List<SimpleBallot>
        {
            // Candidate one: 50%
            new SimpleBallot(new SimpleVote(voterOne, candidateOne)),
            // Candidate two: 50%
            new SimpleBallot(new SimpleVote(voterTwo, candidateTwo)),
        };

        var election = new SimpleElection(ballots, new List<ICandidate>() { candidateOne, candidateTwo });
        
        Assert.Throws<SimpleElectionTie>(() => election.CountVotes());
    }
}