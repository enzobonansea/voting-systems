using Election.Interfaces;
using Election.Objects;
using Election.Objects.Exceptions;

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

    [Fact]
    public void WonByAbsoluteMajorityIsTrueIfAndOnlyIfWinnerHasMoreThan50PercentOfVotes()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidates = new List<ICandidate> { candidateOne, candidateTwo };
        // Candidate one: 2 first-preference votes (66.66%)
        // Candidate two: 1 first-preference votes (33.33%)
        var ballotsWithAbsoluteMajority = new List<RankedChoiceBallot>
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
        Assert.True(new RankedChoiceElectionRound(ballotsWithAbsoluteMajority, candidates).WonByAbsoluteMajority);
        
        // Candidate one: 1 first-preference votes (50%)
        // Candidate two: 1 first-preference votes (50%)
        var ballotsWithoutAbsoluteMajority = new List<RankedChoiceBallot>
        {
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateTwo, 1),
            }),
        };
        Assert.False(new RankedChoiceElectionRound(ballotsWithoutAbsoluteMajority, candidates).WonByAbsoluteMajority);
    }

    [Fact]
    public void NextRoundRemovesCandidateWithFewestFirstPreferenceVotes()
    {
        var voters = VotersFactory.CreateMany(8).ToList();
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        var candidateThree = new SimpleCandidate(3, "Candidate three");
        var candidateFour = new SimpleCandidate(4, "Candidate four");
        var ballots = new List<RankedChoiceBallot>
        {
            // Candidate one: 3 first-preference vote (37,5%)
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
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[2], candidateOne,  1),
                new RankedChoiceVote(voters[2], candidateTwo, 2),
            }),
            // Candidate two: 2 first-preference votes (25%)
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[3], candidateTwo,  1),
                new RankedChoiceVote(voters[3], candidateThree, 2),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[4], candidateTwo,  1),
                new RankedChoiceVote(voters[4], candidateThree, 2),
            }),
            // Candidate three: 2 first-preference votes (25%)
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[5], candidateThree,  1),
                new RankedChoiceVote(voters[5], candidateTwo, 2),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[6], candidateThree,  1),
                new RankedChoiceVote(voters[6], candidateTwo, 2),
            }),
            // Candidate four: 1 first-preference votes (12,5%)
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voters[7], candidateFour,  1),
                new RankedChoiceVote(voters[7], candidateOne, 2),

            }),
        };
        var candidates = new List<ICandidate> { candidateOne, candidateTwo, candidateThree, candidateFour };
        
        var round = new RankedChoiceElectionRound(ballots, candidates);
        round.Next();
        Assert.DoesNotContain(candidateFour, round.Candidates);
        Assert.Equal(4, round.GetFirstPreferenceVotes(candidateOne));
        Assert.Equal(2, round.GetFirstPreferenceVotes(candidateTwo));
        Assert.Equal(2, round.GetFirstPreferenceVotes(candidateThree));
        Assert.False(round.WonByAbsoluteMajority);
        
        round.Next();
        Assert.DoesNotContain(candidateThree, round.Candidates);
        Assert.Equal(4, round.GetFirstPreferenceVotes(candidateOne));
        Assert.Equal(4, round.GetFirstPreferenceVotes(candidateTwo));
        Assert.False(round.WonByAbsoluteMajority);
        
        round.Next();
        Assert.DoesNotContain(candidateTwo, round.Candidates);
        Assert.Equal(4, round.GetFirstPreferenceVotes(candidateOne));
        Assert.True(round.WonByAbsoluteMajority);
        Assert.Equal(candidateOne, round.Winner);
    }
}