namespace P02_FootballBetting;

using P02_FootballBetting.Data;

public class StartUp
{
    static void Main(string[] args)
    {
        var context = new FootballBettingContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
