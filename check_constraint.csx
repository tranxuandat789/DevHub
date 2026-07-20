using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;

var options = new DbContextOptionsBuilder<ItrecruitmentDbContext>()
    .UseSqlServer("Server=localhost;Database=ITRecruitmentDB;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

using (var context = new ItrecruitmentDbContext(options))
{
    var conn = context.Database.GetDbConnection();
    conn.Open();
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "SELECT definition FROM sys.check_constraints WHERE name = 'CK_interview_status'";
        var result = cmd.ExecuteScalar();
        Console.WriteLine("Definition: " + result);
    }
}
