using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirtableApiClient;
using BertScout2022.Data.Models;

namespace BertScout2022.AirTable
{
    public class AirtableDataRoutines
    {
        private const string AIRTABLE_BASE = "appSYiC3Jj92HrBBS";
        private const string AIRTABLE_KEY = "keyIlZIGEOtUMLKSY";

        public async void AirtableSendRecords(List<TeamMatch> matches)
        {
            int RecordCount = 0;
            int NewCount = 0;
            //int UpdatedCount = 0;
            List<Fields> newRecordList = new List<Fields>();
            using (AirtableBase airtableBase = new AirtableBase(AIRTABLE_KEY, AIRTABLE_BASE))
            {
                foreach (TeamMatch match in matches)
                {

                    //List<Fields> newRecordList = new List<Fields>();
                    //List<IdFields> updatedRecordList = new List<IdFields>();
                    //foreach (TeamMatch match in App.Database.GetTeamMatchesAsync().Result)
                    //{
                    //    // only send matches from this event
                    //    if (match.EventKey != App.currFRCEventKey)
                    //    {
                    //        continue;
                    //    }
                    //    // only send matches from this device
                    //    if (match.DeviceName != App.KindleName)
                    //    {
                    //        continue;
                    //    }
                    RecordCount++;
                    if (string.IsNullOrEmpty(match.AirtableId))
                    {
                        Fields fields = new Fields();
                        fields.AddField("Uuid", match.Uuid.ToString());
                        fields.AddField("TeamNumber", match.TeamNumber);
                        fields.AddField("MatchNumber", match.MatchNumber);
                        fields.AddField("ScouterName", match.ScouterName);
                        newRecordList.Add(fields);
                    }
                    //    else
                    //    {
                    //        if (match.Changed % 2 == 0) // even, don't upload
                    //        {
                    //            continue;
                    //        }
                    //        match.Changed++; // make even
                    //        IdFields fields = new IdFields(match.AirtableId.ToString());
                    //        JObject jo = match.ToJson();
                    //        foreach (KeyValuePair<string, object> kv in jo.ToList())
                    //        {
                    //            if (kv.Key == "Id" || kv.Key == "AirtableId")
                    //            {
                    //                continue;
                    //            }
                    //            fields.AddField(kv.Key, kv.Value);
                    //        }
                    //        updatedRecordList.Add(fields);
                    //    }
                }
                if (newRecordList.Count > 0)
                {
                    int tempCount = await AirtableSendNewRecords(airtableBase, newRecordList, matches);
                    if (tempCount < 0)
                    {
                        return; // error, exit out
                    }
                    NewCount += tempCount;
                }
                //if (updatedRecordList.Count > 0)
                //{
                //    int tempCount = await AirtableSendUpdatedRecords(airtableBase, updatedRecordList);
                //    if (tempCount < 0)
                //    {
                //        return; // error, exit out
                //    }
                //    UpdatedCount += tempCount;
                //}
            }
            //Label_Results.Text = $"Records found: {RecordCount}\r\n";
            //Label_Results.Text += $"New records: {NewCount}\r\n";
            //Label_Results.Text += $"Updated records: {UpdatedCount}";
        }

        private async Task<int> AirtableSendNewRecords(AirtableBase airtableBase,
                                                       List<Fields> newRecordList,
                                                       List<TeamMatch> matches)
        {
            AirtableCreateUpdateReplaceMultipleRecordsResponse result;
            List<Fields> sendList = new List<Fields>();
            int finalCount = 0;
            while (newRecordList.Count > 0)
            {
                sendList.Clear();
                do
                {
                    sendList.Add(newRecordList[0]);
                    newRecordList.RemoveAt(0);
                } while (newRecordList.Count > 0 && sendList.Count < 10);
                result = await airtableBase.CreateMultipleRecords("Match", sendList.ToArray());
                if (!result.Success)
                {
                    //Label_Results.Text = "Error uploading:\r\n";
                    //Label_Results.Text += $"{result.AirtableApiError.ErrorMessage}\r\n";
                    return -1;
                }
                foreach (AirtableRecord rec in result.Records)
                {
                    foreach (TeamMatch match in matches)
                    {
                        if (match.Uuid == rec.GetField("Uuid").ToString())
                        {
                            match.AirtableId = rec.Id;
                            finalCount++;
                            break;
                        }
                    }
                }
                if (newRecordList.Count > 0)
                {
                    // can only send 5 batches per second, make sure that doesn't happen
                    System.Threading.Thread.Sleep(500);
                }
            }
            return finalCount;
        }

        async private Task<int> AirtableSendUpdatedRecords(AirtableBase airtableBase,
                                                           List<IdFields> updatedRecordList)
        {
            AirtableCreateUpdateReplaceMultipleRecordsResponse result;
            List<IdFields> sendList = new List<IdFields>();
            int finalCount = 0;
            while (updatedRecordList.Count > 0)
            {
                sendList.Clear();
                do
                {
                    sendList.Add(updatedRecordList[0]);
                    updatedRecordList.RemoveAt(0);
                } while (updatedRecordList.Count > 0 && sendList.Count < 10);
                result = await airtableBase.UpdateMultipleRecords("Match", sendList.ToArray());
                if (!result.Success)
                {
                    Label_Results.Text = "Error uploading:\r\n";
                    Label_Results.Text += $"{result.AirtableApiError.ErrorMessage}\r\n{result.AirtableApiError}";
                    return -1;
                }
                foreach (AirtableRecord rec in result.Records)
                {
                    TeamMatch match = App.Database.GetTeamMatchAsyncUuid(rec.GetField("Uuid").ToString());
                    match.Changed++; // mark as uploaded
                    if (match.Changed % 2 == 1)
                    {
                        match.Changed++; // make even so it doesn't send again
                    }
                    await App.Database.SaveTeamMatchAsync(match);
                    finalCount++;
                }
                if (updatedRecordList.Count > 0)
                {
                    // can only send 5 batches per second, make sure that doesn't happen
                    System.Threading.Thread.Sleep(500);
                }
            }
            return finalCount;
        }

    }
}
