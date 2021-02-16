using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace _4._Add_Minion
{
    class StartUp
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";

        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection= new SqlConnection(ConnectionString);
            sqlConnection.Open();

            StringBuilder output = new StringBuilder();

            string[] minionsInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] minionsInfo = minionsInput[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string minionName = minionsInfo[0];
            string minionAge = minionsInfo[1];
            string minionTown = minionsInfo[2];

            string[] villainInput = Console.ReadLine()
                 .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                 .ToArray();
            string villainName = villainInput[1];
                


            string GetTownIdQueryText = @"SELECT Id FROM Towns WHERE Name = @townName";

            using SqlCommand getTownIdCmd = new SqlCommand(GetTownIdQueryText, sqlConnection);
            getTownIdCmd.Parameters.AddWithValue("@townName", minionTown);
            string townId = getTownIdCmd.ExecuteScalar()?.ToString();

            if(townId == null)
            {
                string insertTownQueryText = @"INSERT INTO Towns (Name) VALUES (@townName)";
                using SqlCommand insertTownCmd = new SqlCommand(insertTownQueryText, sqlConnection);
                insertTownCmd.Parameters.AddWithValue("@townName", minionTown);
                insertTownCmd.ExecuteNonQuery();
                townId = getTownIdCmd.ExecuteScalar()?.ToString();
                output.AppendLine($"Town {minionTown} was added to the database.");
            }

            string getVillainIdQueryText = @"SELECT Id FROM Villains WHERE Name = @VillainName";
            using SqlCommand getVillainIdCmd = new SqlCommand(getVillainIdQueryText, sqlConnection);
            getVillainIdCmd.Parameters.AddWithValue("@villainName", villainName);
            string villainId = getVillainIdCmd.ExecuteScalar()?.ToString();

            if(villainId == null)
            {
                string insertVillainQueryText = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                using SqlCommand insertVillainCmd = new SqlCommand(insertVillainQueryText, sqlConnection);
                insertVillainCmd.Parameters.AddWithValue(@"villainName", villainName);
                insertVillainCmd.ExecuteNonQuery();
                villainId = getVillainIdCmd.ExecuteScalar()?.ToString();
                output.AppendLine($"Villain {villainName} was added to the database.");
            }

            //INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)
            //INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)
            string insertMinionQueryText = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            using SqlCommand insertMinionCmd = new SqlCommand(insertMinionQueryText, sqlConnection);
            insertMinionCmd.Parameters.AddWithValue(@"name", minionName);
            insertMinionCmd.Parameters.AddWithValue( @"age", minionAge);
            insertMinionCmd.Parameters.AddWithValue( @"townId", townId);
            insertMinionCmd.ExecuteNonQuery();
            string minionIdQueryText = @"SELECT Id FROM Minions WHERE Name = @Name";
            using SqlCommand getMinioId = new SqlCommand(minionIdQueryText, sqlConnection);
            getMinioId.Parameters.AddWithValue(@"Name", minionName);
            string minionId = getMinioId.ExecuteScalar()?.ToString();

            string insertMinionVillainQueryText = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
            using SqlCommand insertMinionVillainCmd = new SqlCommand(insertMinionVillainQueryText, sqlConnection);
            insertMinionVillainCmd.Parameters.AddWithValue(@"villainId", villainId);
            insertMinionVillainCmd.Parameters.AddWithValue(@"minionId", minionId);
            insertMinionCmd.ExecuteNonQuery();

            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            Console.WriteLine(output.ToString().TrimEnd());
        }
    }
}
