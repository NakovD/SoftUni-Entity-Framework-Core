namespace ADO.NET_Playground.SQLCommands.Update
{
    public static class Update
    {
        public static string TownsFromCountry = " UPDATE Towns SET [Name] = UPPER([Name]) WHERE Id IN (SELECT T.Id FROM Towns AS T JOIN Countries AS C ON T.CountryCode = C.Id WHERE C.Name = @countryName)";
    }
}
