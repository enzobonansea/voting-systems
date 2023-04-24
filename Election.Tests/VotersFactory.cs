using Election.Interfaces;
using Election.Objects;

namespace Election.Tests;

public static class VotersFactory
{
    public static IEnumerable<IVoter> CreateMany(int votersQuantity)
    {
        return Enumerable.Range(1, votersQuantity + 1).Select(index => new SimpleVoter(index, "Candidate " + index));
    }
}