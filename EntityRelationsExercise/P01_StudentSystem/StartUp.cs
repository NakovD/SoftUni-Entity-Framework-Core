namespace P01_StudentSystem;

using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;
using P01_StudentSystem.Enums;

public class Startup
{
    static void Main(string[] args)
    {
        var context = new StudentSystemContext();

        context.Database.EnsureDeleted();

        context.Database.EnsureCreated();
    }

}
