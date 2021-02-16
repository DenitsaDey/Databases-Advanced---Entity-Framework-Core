using System;
using System.Data.SqlClient;
using System.Text;

namespace _5._Change_Town_Name_Casing
{
    class Program
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";
        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            string countryName = Console.ReadLine();

            string updateTownNamesQuerytext = @"UPDATE Towns
                                                SET Name = UPPER(Name)
                                                WHERE CountryCode = (SELECT c.Id 
                                                                        FROM Countries AS c 
                                                                        WHERE c.Name = @countryName)";
            using SqlCommand updateTowns = new SqlCommand(updateTownNamesQuerytext, sqlConnection);
            updateTowns.Parameters.AddWithValue(@"countryName", countryName);
            int townsAffected = updateTowns.ExecuteNonQuery();

            StringBuilder sb = new StringBuilder();

            string townsUdpdatedQueryText = @"SELECT t.Name 
                                                FROM Towns as t
                                                JOIN Countries AS c ON c.Id = t.CountryCode
                                                WHERE c.Name = @countryName";
            using SqlCommand getUpdatedTowns = new SqlCommand(townsUdpdatedQueryText, sqlConnection);
            getUpdatedTowns.Parameters.AddWithValue(@"countryName", countryName);
            using SqlDataReader townsReader = getUpdatedTowns.ExecuteReader();
            
            if (townsAffected == 0)
            {
                sb.AppendLine("No town names were affected.");
            }
            else
            {
                
                sb.AppendLine($"{townsAffected} town names were affected.");
                sb.Append("[");
                int counter = 0;
                while (townsReader.Read())
                {
                    sb.Append($"{townsReader["Name"]?.ToString()}");
                    if(counter != townsAffected - 1)
                    {
                        sb.Append(",");
                    }
                    counter++;
                }
                
                sb.AppendLine("]");
            }
            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}
