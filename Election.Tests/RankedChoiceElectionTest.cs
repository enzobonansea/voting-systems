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
    
    [Fact]
    public void BallotsVotesMustHaveDifferentRanks()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo, 1)
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };

        Assert.Throws<BallotsVotesMustHaveDifferentRanks>(() => new RankedChoiceElection(ballots, candidates));
    }

    
    [Fact]
    public void BallotsMustHaveRanksBetweenOneAndVotesLength()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };
        var ballotsWithTooBigRank = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo, 2)
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo, 3)
            }),
        };
        var ballotsWithTooLittleRank = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo, 2)
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 0),
                new RankedChoiceVote(voterOne, candidateTwo, 2)
            }),
        };

        Assert.Throws<BallotsMustHaveRanksBetweenOneAndVotesLength>(() => new RankedChoiceElection(ballotsWithTooBigRank, candidates));
        Assert.Throws<BallotsMustHaveRanksBetweenOneAndVotesLength>(() => new RankedChoiceElection(ballotsWithTooLittleRank, candidates));
    }
    
    [Fact]
    public void PeopleCannotVoteMoreThanOnce()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne };
        
        Assert.Throws<PeopleCannotVoteMoreThanOnce>(() => new RankedChoiceElection(ballots, candidates));
    }
    
    [Fact]
    public void VotedCandidatesMustBeValid()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, new SimpleCandidate(2, "Candidate two"), 1),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne };
        
        Assert.Throws<InvalidCandidate>(() => new RankedChoiceElection(ballots, candidates));
    }

    [Fact]
    public void BallotsMustHaveSameVoter()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate tw0");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterTwo, candidateTwo,  2),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };
        
        Assert.Throws<BallotsMustHaveSameVoter>(() => new RankedChoiceElection(ballots, candidates));
    }
    
    [Fact]
    public void BallotsMustHaveDifferentCandidates()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateOne,  2),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne };
        
        Assert.Throws<BallotsMustHaveDifferentCandidates>(() => new RankedChoiceElection(ballots, candidates));
    }

    [Fact]
    public void AbsoluteMajorityWins()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
                new RankedChoiceVote(voterOne, candidateTwo,  2),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
                new RankedChoiceVote(voterTwo, candidateTwo,  2),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterThree, candidateTwo,  1),
                new RankedChoiceVote(voterThree, candidateOne, 2),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };
        var election = new RankedChoiceElection(ballots, candidates);
        election.CountVotes();
        Assert.Equal(candidateOne, election.Winner);
    }
}