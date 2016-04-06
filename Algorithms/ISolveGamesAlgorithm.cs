namespace Algorithms
{
    public interface ISolveGamesAlgorithm
    {
        Game SolveGame(ScheduleItem item, GamePlayer[] playersAvailableForThisGame);
    }
}