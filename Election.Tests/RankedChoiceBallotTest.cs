using Election.Objects;

namespace Election.Tests;

public class RankedChoiceBallotTest
{
    [Fact]
    public void RemoveCandidateRemovesTheirVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateOneVote = new RankedChoiceVote(voterOne, candidateOne, 1);
        var candidateTwoVote = new RankedChoiceVote(voterOne, candidateTwo, 2);
        var ballot = new RankedChoiceBallot(new List<RankedChoiceVote>
        {
            candidateOneVote,
            candidateTwoVote
        });
        ballot.Remove(candidateTwo);
        Assert.DoesNotContain(candidateTwoVote, ballot.Votes);
    }
    
    [Fact]
    public void RemoveCandidateModifyOtherCandidatesRank()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateThree = new SimpleCandidate(2, "Candidate two");
        var candidateFour = new SimpleCandidate(2, "Candidate two");
        var candidateOneVote = new RankedChoiceVote(voterOne, candidateOne, 1);
        var candidateTwoVote = new RankedChoiceVote(voterOne, candidateTwo, 2);
        var candidateThreeVote = new RankedChoiceVote(voterOne, candidateThree, 3);
        var candidateFourVote = new RankedChoiceVote(voterOne, candidateFour, 4);
        var ballot = new RankedChoiceBallot(new List<RankedChoiceVote>
        {
            candidateOneVote, candidateTwoVote, candidateThreeVote, candidateFourVote
        });
        ballot.Remove(candidateTwo);
        Assert.Equal(1, ballot.Votes.First(vote => vote.Candidate == candidateOne).Rank);
        Assert.Equal(2, ballot.Votes.First(vote => vote.Candidate == candidateThree).Rank);
        Assert.Equal(3, ballot.Votes.First(vote => vote.Candidate == candidateFour).Rank);
    }
    
    [Fact]
    public void HasReturnsTrueIfAndOnlyIfCandidateIsInBallot()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateOneVote = new RankedChoiceVote(voterOne, candidateOne, 1);
        var ballot = new RankedChoiceBallot(new List<RankedChoiceVote>
        {
            candidateOneVote,
        });
        Assert.True(ballot.Has(candidateOne));
        Assert.False(ballot.Has(candidateTwo));
    }
}