using System;
using System.Data.SqlClient;

namespace _6._Remove_Villain
{
    class Program
    {
        private const string ConnectionString = @"Server=LAPTOP-9JDUN69F;Database=MinionsDB; Integrated Security=true;";
        public static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int villainID = int.Parse(Console.ReadLine());

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            string villainNamesQuerytext = @"SELECT Name FROM Villains WHERE Id = @villainId";
            using SqlCommand getVillainName = new SqlCommand(villainNamesQuerytext, sqlConnection);
            getVillainName.Parameters.AddWithValue(@"villainId", villainID);

            getVillainName.Transaction = sqlTransaction;

            string villain = getVillainName.ExecuteScalar()?.ToString();

            if(villain == null)
            {
                Console.WriteLine("No such villain was found.");
            }
            else
            {
                try
                {
                    string deleteVilainFromMVQueryText = @"DELETE FROM MinionsVillains 
                                                         WHERE VillainId = @villainId";
                    using SqlCommand deleteFromMV = new SqlCommand(deleteVilainFromMVQueryText, sqlConnection);
                    deleteFromMV.Parameters.AddWithValue(@"villainId", villainID);

                    deleteFromMV.Transaction = sqlTransaction;

                    int minionsDeleted = deleteFromMV.ExecuteNonQuery();

                    string deleteVillainfromVillainsQuerytext = @"DELETE FROM Villains 
                                                                WHERE Id = @villainId";
                    using SqlCommand deleteVillain = new SqlCommand(deleteVillainfromVillainsQuerytext, sqlConnection);
                    deleteVillain.Parameters.AddWithValue(@"villainId", villainID);

                    deleteVillain.Transaction = sqlTransaction;

                    deleteVillain.ExecuteNonQuery();

                    sqlTransaction.Commit();

                    Console.WriteLine($"{villain} was deleted.");
                    Console.WriteLine($"{minionsDeleted} minions were released.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    try
                    {
                        sqlTransaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        Console.WriteLine(rollbackEx.Message);
                    }
                    
                }
            }
        }
    }
}
