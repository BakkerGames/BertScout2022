using SQLite;
using System;

namespace BertScout2022.Data
{
    public class BertScout2022Database
    {
        public const string dbFilename = "bertscout2022.db3";

        // update when db structure changes
        public const decimal dbVersion = 0.1M;

        private static SQLiteAsyncConnection _database;

        public BertScout2022Database(string dbPath)
        {
            try
            {
                _database = new SQLiteAsyncConnection(dbPath);
                CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string CreateTables()
        {
            try
            {
                //_database.CreateTableAsync<FRCEvent>().Wait();
                //_database.CreateTableAsync<Team>().Wait();
                //_database.CreateTableAsync<EventTeam>().Wait();
                //_database.CreateTableAsync<EventTeamMatch>().Wait();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DropTables()
        {
            try
            {
                //_database.DropTableAsync<FRCEvent>().Wait();
                //_database.DropTableAsync<Team>().Wait();
                //_database.DropTableAsync<EventTeam>().Wait();
                //_database.DropTableAsync<EventTeamMatch>().Wait();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
