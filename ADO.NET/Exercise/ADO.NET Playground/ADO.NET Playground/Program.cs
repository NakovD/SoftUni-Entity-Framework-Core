﻿using ADO.NET_Playground.SQLCommands.Create;
using ADO.NET_Playground.SQLCommands.Insert;
using ADO.NET_Playground.SQLCommands.Queries;
using ADO.NET_Playground.SQLCommands.Update;
using Microsoft.Data.SqlClient;

internal class Program
{
    private static string connectionString = "Server=.;Database=MinionsDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";

    private static async Task Main(string[] args)
    {
        var connection = new SqlConnection(connectionString);

        connection.Open();

        using (connection)
        {
        }
    }

    private static async Task CreateTables(SqlConnection connection)
    {
        var createCountries = new SqlCommand(Create.Contries, connection);
        await createCountries.ExecuteNonQueryAsync();

        var createTowns = new SqlCommand(Create.Towns, connection);
        await createTowns.ExecuteNonQueryAsync();

        var createMinions = new SqlCommand(Create.Minions, connection);
        await createMinions.ExecuteNonQueryAsync();

        var createEvilnessFactors = new SqlCommand(Create.EvilnessFactors, connection);
        await createEvilnessFactors.ExecuteNonQueryAsync();

        var createVillains = new SqlCommand(Create.Villains, connection);
        await createVillains.ExecuteNonQueryAsync();

        var createMinionsVillains = new SqlCommand(Create.MinionsVillains, connection);
        await createMinionsVillains.ExecuteNonQueryAsync();
    }

    private static async Task GetVillanNames(SqlConnection connection)
    {
        var villainNamesCommand = new SqlCommand(Queries.VillainNames, connection);
        SqlDataReader result = await villainNamesCommand.ExecuteReaderAsync();

        while (result.Read())
        {
            Console.WriteLine($"{result[0]} - {result[1]}");
        }
    }

    private static async Task GetVillanMinions(SqlConnection connection, int villanId)
    {
        var villanExistsCommand = new SqlCommand(Queries.VillianExistsQuery, connection);

        villanExistsCommand.Parameters.AddWithValue("@id", villanId);

        var villanExistsResult = await villanExistsCommand.ExecuteReaderAsync();

        if (!villanExistsResult.HasRows)
        {
            Console.WriteLine($"No villain with ID {villanId} exists in the database.");
            return;
        }

        villanExistsResult.Read();

        var villainNamesCommand = new SqlCommand(Queries.VillainMinionsNames, connection);

        villainNamesCommand.Parameters.AddWithValue("@id", villanId);
        var result = await villainNamesCommand.ExecuteReaderAsync();

        Console.WriteLine($"villan: {villanExistsResult["Name"]}");

        if (!result.HasRows)
        {
            Console.WriteLine("(no minions)");
            return;
        }

        var counter = 1;

        while (result.Read())
        {
            Console.WriteLine($"{counter}. {result["Name"]} - {result["Age"]}");
            counter++;
        }
    }

    private static async Task InsertMinionInDb(SqlConnection connection, string minionName, int minionAge, string minionTown, string villanName)
    {
        var townCommand = new SqlCommand(Queries.Town, connection);

        townCommand.Parameters.AddWithValue("@townName", minionTown);

        var townResult = await townCommand.ExecuteReaderAsync();

        if (!townResult.HasRows)
        {
            await AddTownToDb(connection, minionTown);

            await townResult.CloseAsync();

            townResult = await townCommand.ExecuteReaderAsync();
        }

        var villanCommand = new SqlCommand(Queries.Villain, connection);

        villanCommand.Parameters.AddWithValue("@villianName", villanName);

        var villanResult = await villanCommand.ExecuteReaderAsync();

        if (!villanResult.HasRows)
        {
            await AddVillanToDB(connection, villanName);

            await villanResult.CloseAsync();

            villanResult = await villanCommand.ExecuteReaderAsync();
        }
        townResult.Read();

        villanResult.Read();

        var townId = townResult["Id"];

        var villanId = villanResult["Id"];

        var finalQuery = Insert.Minion + "; " + Queries.ScopeIdentity;

        var minionCommand = new SqlCommand(finalQuery, connection);

        minionCommand.Parameters.AddWithValue("@minionName", minionName);
        minionCommand.Parameters.AddWithValue("@minionAge", minionAge);
        minionCommand.Parameters.AddWithValue("@minionTownId", townId);

        var result = await minionCommand.ExecuteReaderAsync();

        result.Read();

        var insertedMinionId = result[0];

        var minnionsVillainsCommand = new SqlCommand(Insert.MinionVillans, connection);

        minnionsVillainsCommand.Parameters.AddWithValue("@minionId", insertedMinionId);
        minnionsVillainsCommand.Parameters.AddWithValue("@villainId", villanId);

        await minnionsVillainsCommand.ExecuteNonQueryAsync();

        Console.WriteLine($"Successfully added {minionName} to be minion of {villanName}.");
    }

    private static async Task UpdateTownNames(SqlConnection connection, string country)
    {
        SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            var townsCommand = new SqlCommand(Update.TownsFromCountry, connection, transaction);

            townsCommand.Parameters.AddWithValue("@countryName", country);

            int townsResult = await townsCommand.ExecuteNonQueryAsync();

            if (townsResult != 0)
            {
                Console.WriteLine($"{townsResult} town names were affected.");

                var townsUpdatedCommand = new SqlCommand(Queries.TownsFromCountry, connection, transaction);

                townsUpdatedCommand.Parameters.AddWithValue("@countryName", country);

                var townsUpdateResult = await townsUpdatedCommand.ExecuteReaderAsync();

                var towns = new List<string>();

                while (townsUpdateResult.Read())
                {
                    towns.Add(townsUpdateResult["Name"].ToString());
                }

                var townsString = string.Join(", ", towns);

                Console.WriteLine($"[{townsString}]");
            }
            else Console.WriteLine("No town names were affected.");

            transaction.Rollback();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }
    }

    private static async Task AddVillanToDB(SqlConnection connection, string villanName)
    {
        var villanCommand = new SqlCommand(Insert.Villain, connection);

        villanCommand.Parameters.AddWithValue("@villianName", villanName);

        await villanCommand.ExecuteNonQueryAsync();

        Console.WriteLine($"Villain {villanName} was added to the database.");
    }

    private static async Task AddTownToDb(SqlConnection connection, string minionTown)
    {
        var townCommand = new SqlCommand(Insert.Town, connection);

        townCommand.Parameters.AddWithValue("@townName", minionTown);

        await townCommand.ExecuteNonQueryAsync();

        Console.WriteLine($"Town {minionTown} was added to the database.");
    }
}