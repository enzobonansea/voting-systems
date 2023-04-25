# Voting systems
This is my solution to the following challenge:

Given the recent buzz around the New York City mayoral election and its use of ranked-choice voting (a basic explanation of which can be found here), we have created a project to simulate the ballot counting and show how a majority winner would not necessarily win a ranked-choice election. The task is to implement SimpleElection.CountVotes() and RankedChoiceElection.CountVotes() in such a way that their respective ElectionRunners will return the correct winner of the election based on the voting system (you may also want to take a look at ElectionRunner.cs). Changes can be made to anything in the solution, including the BallotGenerators, which currently just generate 100,000 random votes. The submission should include a short explanation of your solution and any unit tests you used along the way.

The codebase provided is [here](https://github.com/enzobonansea/voting-systems/commit/fc4c2246692940f15a312e4de40dd52b73b276e4)

I used Test-Driven Development in every line of code added, and all commits follow [this convention](https://www.conventionalcommits.org/en/v1.0.0/) with some personal variations. In the beginning, I pushed first the test and then the feature in order to emphasize TDD usage, but then I pushed both the test and feature together for simplicity.
