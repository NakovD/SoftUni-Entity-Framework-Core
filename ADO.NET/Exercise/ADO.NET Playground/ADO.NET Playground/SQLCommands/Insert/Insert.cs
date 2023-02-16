namespace ADO.NET_Playground.SQLCommands.Insert
{
    public static class Insert
    {
        public static string Town = "INSERT INTO Towns (Name) VALUES(@townName)";

        public static string Villain = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES(@villianName, 4)";

        public static string Minion = "INSERT INTO Minions (Name, Age, TownId) VALUES(@minionName, @minionAge, @minionTownId)";

        public static string MinionVillans = "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(@minionId, @villainId)";
    }
}
