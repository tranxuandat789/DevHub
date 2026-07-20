using System;
using System.Data.SqlClient;

string connStr = "Server=localhost;Database=IT_Recruitment_DevHub;Trusted_Connection=True;TrustServerCertificate=True;";
using (SqlConnection conn = new SqlConnection(connStr))
{
    conn.Open();
    using (SqlCommand cmd = new SqlCommand("SELECT object_definition(object_id) as definition FROM sys.objects WHERE type = 'C' OR type = 'TR';", conn))
    {
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
