using System;
using System.Data.SqlClient;
using System.Linq;

namespace _8._Increase_Minion_Age
{
    class Program
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";

        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int[] minionIds = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            

            for (int i = 0; i < minionIds.Length; i++)
            {
                string QueryText = @"UPDATE Minions
                                 SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                 WHERE Id = @Id";
                using SqlCommand IncrementAge = new SqlCommand(QueryText, sqlConnection);
                IncrementAge.Parameters.AddWithValue(@"Id", minionIds[i]);
                IncrementAge.ExecuteNonQuery();
            }

            string PrintAllQueryText = @"SELECT Name, Age FROM Minions";
            using SqlCommand PrintAllCmd = new SqlCommand(PrintAllQueryText, sqlConnection);
            using SqlDataReader reader = PrintAllCmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]?.ToString()} {reader["Age"]?.ToString()}");
            }
        }
    }
}
