using System;
using System.Data.SqlClient;
using System.Text;

namespace _3._Minion_Names
{
    class StartUp
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";

        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            StringBuilder sb = new StringBuilder();

            int villainId = int.Parse(Console.ReadLine());

            string getVillainNameQueryText = @"SELECT Name FROM Villains WHERE Id = @villainId";

            using SqlCommand getVillainNameCmd = new SqlCommand(getVillainNameQueryText, sqlConnection);

            getVillainNameCmd.Parameters.AddWithValue("@villainId", villainId);

            string villainName = getVillainNameCmd.ExecuteScalar()?.ToString();

            if (villainName == null)
            {
                sb.AppendLine($"No villain with ID {villainId} exists in the database.");
            }
            else
            {
                sb.AppendLine($"Villain: {villainName}");
                string getMinionsInfoQueryText = @"SELECT m.Name, m.Age 
	                                                FROM MinionsVillains AS mv
	                                                JOIN Minions AS m ON mv.MinionId = m.Id
	                                                WHERE VillainId = @villainId
	                                                ORDER BY m.Name";

                SqlCommand getMinionsNameAndAgeCmd = new SqlCommand(getMinionsInfoQueryText, sqlConnection);

                getMinionsNameAndAgeCmd.Parameters.AddWithValue("@villainId", villainId);

                using SqlDataReader reader = getMinionsNameAndAgeCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    int rowNum = 1;
                    while (reader.Read())
                    {
                        sb.AppendLine($"{rowNum}. {reader["Name"]?.ToString()} {reader["Age"]?.ToString()}");
                        rowNum++;

                    }
                }
                else
                {
                    sb.AppendLine("(no minions)");
                }

                Console.WriteLine(sb.ToString().TrimEnd());
            }
        }
    }
}
