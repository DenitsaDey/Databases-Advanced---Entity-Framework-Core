using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace _7._Print_All_Minion_Names
{
    class Program
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";
        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            string QueryText = @"SELECT Name FROM Minions";
            using SqlCommand GetMinionNames = new SqlCommand(QueryText, sqlConnection);
            using SqlDataReader minionsReader = GetMinionNames.ExecuteReader();

            List<string> minions = new List<string>();

            while (minionsReader.Read())
            {
                minions.Add(minionsReader["Name"]?.ToString());
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < minions.Count/2; i++)
            {
                sb.AppendLine(minions[i]);
                sb.AppendLine(minions[minions.Count-1 - i]);
            }

            if(minions.Count % 2 != 0)
            {
                sb.AppendLine(minions[minions.Count / 2]);
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}
