namespace ADO.NET_Playground.SQLCommands.Delete
{
    public static class Delete
    {
        public static string DeleteVillainMinions = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";

        public static string DeleteVillainById = "DELETE FROM Villains WHERE Id = @villainId";
    }
}
