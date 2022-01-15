using BertScout2022.Data.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BertScout2022.Data
{
    public class BertScout2022Database
    {
        // update when db structure changes
        public const decimal dbVersion = 0.4M;

        public const string dbFilename = "bertscout2022.db3";

        private static SQLiteAsyncConnection _database;

        public BertScout2022Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            CreateTables();
        }

        public async void CreateTables()
        {
            await _database.CreateTableAsync<TeamMatch>();
        }

        public async void DropTables()
        {
            await _database.DropTableAsync<TeamMatch>();
        }

        public async void ClearTables()
        {
            await _database.ExecuteAsync("TRUNCATE TABLE [TeamMatch];");
        }

        // TeamMatch

        public async Task<List<TeamMatch>> GetTeamMatchesAsync()
        {
            return await _database.Table<TeamMatch>().ToListAsync();
        }

        public async Task<TeamMatch> GetTeamMatchAsync(int teamNumber, int matchNumber)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT [TeamMatch].* FROM [TeamMatch]");
            query.Append(" WHERE [TeamMatch].[TeamNumber] = ");
            query.Append(teamNumber);
            query.Append(" AND [TeamMatch].[MatchNumber] = ");
            query.Append(matchNumber);
            List<TeamMatch> result = await _database.QueryAsync<TeamMatch>(query.ToString());
            if (result == null || result.Count == 0)
            {
                return null;
            }
            return result[0];
        }

        public async Task<int> SaveTeamMatchAsync(TeamMatch item)
        {
            if (item.Uuid == null)
            {
                TeamMatch existingItem = await GetTeamMatchAsync(item.TeamNumber, item.MatchNumber);
                if (existingItem != null)
                {
                    throw new Exception($"Item already exists - team={item.TeamNumber}, match={item.MatchNumber}");
                }
                item.Uuid = Guid.NewGuid().ToString();
            }
            return await _database.InsertOrReplaceAsync(item);
        }

        public async Task<int> DeleteTeamMatchAsync(TeamMatch item)
        {
            if (item.Uuid == null)
            {
                return 0;
            }
            return await _database.DeleteAsync(item);
        }
    }
}
