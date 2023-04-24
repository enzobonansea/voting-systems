using Election.Interfaces;
using Election.Objects;

namespace Election.Tests.Factories;

public static class RankedChoiceElectionFactory
{
    public static (IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates, ICandidate winner) WithAbsoluteMajorityInFirstPreference()
    {
        var voterOne = new SimpleVoter(1, "Voter one");
        var voterTwo = new SimpleVoter(2, "Voter two");
        var voterThree = new SimpleVoter(3, "Voter three");
        var candidateOne = new SimpleCandidate(1, "Candidate one");
        var candidateTwo = new SimpleCandidate(2, "Candidate two");
        
        var ballots = new List<RankedChoiceBallot>
        {
            // Candidate one: 2 first-preference votes (66.66%)
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterOne, candidateOne, 1),
            }),
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterTwo, candidateOne, 1),
            }),
            // Candidate two: 1 first-preference votes (33.33%)
            new RankedChoiceBallot(new List<RankedChoiceVote>
            {
                new RankedChoiceVote(voterThree, candidateTwo,  1),
            }),
        };
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo };

        return (ballots, candidates, candidateOne);
    }
    
    /// <summary>
    /// Since there isn't an absolute first-preference majority, there isn't a winner in first round
    /// and the candidate with fewest first-preference votes is eliminated (candidate four). Then,
    /// the first-preference configurations are:
    /// * Candidate one: 4 first-preference vote (50%)
    /// * Candidate two: 2 first-preference votes (25%)
    /// * Candidate three: 2 first-preference votes (25%)
    /// Again, there isn't an absolute first-preference majority and the candidate with fewest first-preference 
    /// votes is eliminated (candidate three since RankedChoiceElectionRound.Loser is stable). The new configurations are:
    /// * Candidate one: 4 first-preference vote (50%)
    /// * Candidate two: 4 first-preference vote (50%)
    /// Again, there isn't an absolute first-preference majority and the candidate with fewest first-preference 
    /// votes is eliminated (candidate two since RankedChoiceElectionRound.Loser is stable). The new configurations are:
    /// * Candidate one: 4 first-preference vote (50%)
    /// And thus, the winner is candidate one with 4 first-preference votes
    /// </summary>
    /// <returns></returns>
    public static (IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates, ICandidate winner)  WithoutAbsoluteMajorityInFirstPreference()
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
        var candidates = new List<ICandidate>() { candidateOne, candidateTwo, candidateThree, candidateFour };

        return (ballots, candidates, candidateOne);
    }
}