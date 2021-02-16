using System;
using System.Data.SqlClient;

namespace _9._Increase_Age_Stored_Procedure
{
    public class Program
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";

        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int minionId = int.Parse(Console.ReadLine());

            string ProcedureName = "usp_GetOlder";
            using SqlCommand increaseAgeCmd = new SqlCommand(ProcedureName, sqlConnection);
            increaseAgeCmd.CommandType = System.Data.CommandType.StoredProcedure;
            increaseAgeCmd.Parameters.AddWithValue("@Id", minionId);
            increaseAgeCmd.ExecuteNonQuery();

            string getMinionInfoQueryText = @"SELECT Name, Age FROM Minions WHERE Id = @Id";
            using SqlCommand getMinionInfoCmd = new SqlCommand(getMinionInfoQueryText, sqlConnection);
            getMinionInfoCmd.Parameters.AddWithValue("@Id", minionId);
            using SqlDataReader reader = getMinionInfoCmd.ExecuteReader();

            reader.Read();

            Console.WriteLine($"{reader["Name"]?.ToString()} - {reader["Age"]?.ToString()} yeras old");
        }
    }
}
