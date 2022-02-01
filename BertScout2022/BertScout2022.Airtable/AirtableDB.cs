using AirtableApiClient;
using BertScout2022.Data.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BertScout2022.Airtable
{
    public class AirtableDB
    {
        private const string AIRTABLE_BASE = "appSYiC3Jj92HrBBS";
        private const string AIRTABLE_KEY = "keyIlZIGEOtUMLKSY";

        public static async Task<string> AirtableSendRecords(List<TeamMatch> matches)
        {
            StringBuilder result = new StringBuilder();
            int NewCount = 0;
            int UpdatedCount = 0;
            List<Fields> newRecordList = new List<Fields>();
            List<IdFields> updatedRecordList = new List<IdFields>();
            FieldInfo[] myFieldInfo;
            Type myType = typeof(TeamMatch);
            myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            using (AirtableBase airtableBase = new AirtableBase(AIRTABLE_KEY, AIRTABLE_BASE))
            {
                foreach (TeamMatch match in matches)
                {
                    if (match.Uuid == null) continue;
                    if (string.IsNullOrEmpty(match.AirtableId))
                    {
                        Fields fields = new Fields();
                        foreach (FieldInfo fi in myFieldInfo)
                        {
                            // name is "<name>stuff", so just get the name part
                            int pos1 = fi.Name.IndexOf('<') + 1;
                            int pos2 = fi.Name.IndexOf('>');
                            string name = fi.Name.Substring(pos1, pos2 - pos1);
                            // these fields are not in airtable
                            if (name.ToLower() == "id") continue;
                            if (name.ToLower() == "airtableid") continue;
                            if (name.ToLower() == "changed") continue;
                            fields.AddField(name, fi.GetValue(match));
                        }
                        newRecordList.Add(fields);
                    }
                    else if (match.Changed)
                    {
                        IdFields idFields = new IdFields(match.AirtableId);
                        foreach (FieldInfo fi in myFieldInfo)
                        {
                            // name is "<name>stuff", so just get the name part
                            int pos1 = fi.Name.IndexOf('<') + 1;
                            int pos2 = fi.Name.IndexOf('>');
                            string name = fi.Name.Substring(pos1, pos2 - pos1);
                            // these fields are not in airtable
                            if (name.ToLower() == "id") continue;
                            if (name.ToLower() == "airtableid") continue;
                            if (name.ToLower() == "changed") continue;
                            idFields.AddField(name, fi.GetValue(match));
                        }
                        updatedRecordList.Add(idFields);
                    }
                }
                if (newRecordList.Count > 0)
                {
                    int tempCount = await AirtableSendNewRecords(airtableBase, newRecordList, matches);
                    if (tempCount < 0)
                    {
                        result.AppendLine("Error!");
                        return result.ToString(); // error, exit out
                    }
                    NewCount += tempCount;
                }
                if (updatedRecordList.Count > 0)
                {
                    int tempCount = await AirtableSendUpdatedRecords(airtableBase, updatedRecordList);
                    if (tempCount < 0)
                    {
                        result.AppendLine("Error!");
                        return result.ToString(); // error, exit out
                    }
                    UpdatedCount += tempCount;
                }
            }
            if (NewCount > 0) result.AppendLine($"Records added to Airtable: {NewCount}");
            if (UpdatedCount > 0) result.AppendLine($"Records updated on Airtable: {UpdatedCount}");
            if (NewCount + UpdatedCount == 0) result.AppendLine("No changes, nothing sent to Airtable");
            return result.ToString();
        }

        private static async Task<int> AirtableSendNewRecords(AirtableBase airtableBase,
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
                result = await airtableBase.CreateMultipleRecords("TeamMatch", sendList.ToArray());
                if (!result.Success)
                {
                    return -1;
                }
                foreach (AirtableRecord rec in result.Records)
                {
                    foreach (TeamMatch match in matches)
                    {
                        if (match.Uuid == rec.GetField("Uuid")?.ToString())
                        {
                            match.AirtableId = rec.Id;
                            finalCount++;
                            break;
                        }
                    }
                }
                if (newRecordList.Count > 0)
                {
                    // can only send 5 batches per second - make sure that doesn't happen
                    System.Threading.Thread.Sleep(500);
                }
            }
            return finalCount;
        }

        private static async Task<int> AirtableSendUpdatedRecords(AirtableBase airtableBase,
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
                result = await airtableBase.UpdateMultipleRecords("TeamMatch", sendList.ToArray());
                if (!result.Success)
                {
                    return -1;
                }
                foreach (AirtableRecord rec in result.Records)
                {
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
