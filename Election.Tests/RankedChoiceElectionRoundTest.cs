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

    [Fact]
    public void WinnerIsWhoHaveMoreFirstPreferenceVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidates = new List<ICandidate> { candidateOne, candidateTwo };
        // Candidate one: 2 first-preference votes (66.66%)
        // Candidate two: 1 first-preference votes (33.33%)
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterThree, candidateTwo,  1),
            }),
        };
        var round = new RankedChoiceElectionRound(ballots, candidates);
        Assert.Equal(candidateOne, round.Winner);
    }
    
    [Fact]
    public void LoserIsWhoHaveLessFirstPreferenceVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidates = new List<ICandidate> { candidateOne, candidateTwo };
        // Candidate one: 2 first-preference votes (66.66%)
        // Candidate two: 1 first-preference votes (33.33%)
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterThree, candidateTwo,  1),
            }),
        };
        var round = new RankedChoiceElectionRound(ballots, candidates);
        Assert.Equal(candidateTwo, round.Loser);
    }

    [Fact]
    public void OnTieLoserIsLast()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateThree = new SimpleCandidate(3, "Candidate three");
        var candidates = new List<ICandidate> { candidateOne, candidateTwo, candidateThree };
        // Candidate one: 2 first-preference votes (66.66%)
        // Candidate two: 1 first-preference votes (33.33%)
        var ballots = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterThree, candidateTwo,  1),
            }),
        };
        var round = new RankedChoiceElectionRound(ballots, candidates);
        Assert.Equal(candidateThree, round.Loser);
    }
}