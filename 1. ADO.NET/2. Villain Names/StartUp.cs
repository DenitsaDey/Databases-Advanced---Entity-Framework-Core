using System;
using System.Data.SqlClient;

namespace _2._Villain_Names
{
    class StartUp
    {
        private const string ConnectionStr = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security = true;";
        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionStr);

            sqlConnection.Open();

            string sqlGetVillainNamesQueryTest = @"SELECT v.Name, COUNT(mv.VillainId) AS [Number of Minions]
                                                    FROM Villains AS v
                                                    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                                    GROUP BY v.Name
                                                    HAVING COUNT(mv.VillainId) > 3
                                                    ORDER BY COUNT(mv.VillainId)";

            using SqlCommand sqlGetResultCmd = new SqlCommand(sqlGetVillainNamesQueryTest, sqlConnection);

            using SqlDataReader reader = sqlGetResultCmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]?.ToString()} - {reader["Number of Minions"]?.ToString()}");
            }

        }
    }
}
