namespace ADO.NET_Playground.SQLCommands.Update
{
    public static class Update
    {
        public static string TownsFromCountry = "UPDATE Towns SET [Name] = UPPER([Name]) WHERE Id IN (SELECT T.Id FROM Towns AS T JOIN Countries AS C ON T.CountryCode = C.Id WHERE C.Name = @countryName)";
        
        public static string MinionNameAndAge = "UPDATE Minions SET Name = LOWER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1 WHERE Id = @minionId";

        public static string MinionAgeWithStoredProcedure = "EXEC usp_GetOlder @minionId";
    }
}
