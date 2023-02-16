namespace ADO.NET_Playground.SQLCommands.Queries
{
    public static class Queries
    {
        public static string VillainNames = "SELECT V.Name, COUNT(MV.MinionId) AS [Count] FROM Villains AS V JOIN MinionsVillains AS MV ON V.Id = MV.VillainId GROUP BY V.Name HAVING COUNT(MV.MinionId) > 3 ORDER BY [Count] DESC";

        public static string VillianExistsQuery = "SELECT Name FROM Villains WHERE Id = @id";

        public static string VillainMinionsNames = "SELECT M.Name, M.Age FROM Villains AS V JOIN MinionsVillains AS MV ON V.Id = MV.VillainId JOIN Minions AS M ON MV.MinionId = M.Id WHERE V.Id = @id ORDER BY M.Name";

        public static string Town = "SELECT * FROM Towns WHERE Name = @townName";

        public static string Villain = "SELECT * FROM Villains WHERE Name = @villianName";

        public static string ScopeIdentity = "SELECT SCOPE_IDENTITY()";

        public static string TownsFromCountry = "SELECT * FROM Towns AS T JOIN Countries AS C ON T.CountryCode = C.Id WHERE C.Name = @countryName";

    }
}
