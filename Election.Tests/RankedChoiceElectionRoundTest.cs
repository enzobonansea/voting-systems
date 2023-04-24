using Election.Interfaces;
using Election.Objects;

namespace Election.Tests;

public class RankedChoiceElectionRoundTest
{
    [Fact]
    public void GetFirstPreferenceVotesWhenCandidateHasFirstPreferenceVotes()
    {
        var voters = VotersFactory.CreateMany(2).ToList();
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidates = new List<ICandidate>{ candidateOne };
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[0], candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[1], candidateOne, 1),
            }),
        };
        var round = new RankedChoiceElectionRound(ballots, candidates);
        Assert.Equal(2, round.GetFirstPreferenceVotes(candidateOne));
    }
    
    [Fact]
    public void GetFirstPreferenceVotesWhenCandidateHasNotFirstPreferenceVotes()
    {
        var voters = VotersFactory.CreateMany(2).ToList();
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidates = new List<ICandidate>{ candidateOne };
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[0], candidateOne, 1),
                new RankedChoiceVote(voters[0], candidateTwo, 2),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[1], candidateOne, 1),
                new RankedChoiceVote(voters[1], candidateTwo, 2),
            }),
        };
        var round = new RankedChoiceElectionRound(ballots, candidates);
        Assert.Throws<CandidateHasNotFirstPreferenceVotes>(() => round.GetFirstPreferenceVotes(candidateTwo));
    }
}