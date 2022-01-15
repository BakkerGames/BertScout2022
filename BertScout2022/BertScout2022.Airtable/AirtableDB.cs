using AirtableApiClient;
using BertScout2022.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BertScout2022.Airtable
{
    public class AirtableDB
    {
        private const string AIRTABLE_BASE = "appSYiC3Jj92HrBBS";
        private const string AIRTABLE_KEY = "keyIlZIGEOtUMLKSY";

        public static async Task AirtableSendRecords(List<TeamMatch> matches)
        {
            int RecordCount = 0;
            int NewCount = 0;
            //int UpdatedCount = 0;
            List<Fields> newRecordList = new List<Fields>();
            using (AirtableBase airtableBase = new AirtableBase(AIRTABLE_KEY, AIRTABLE_BASE))
            {
                foreach (TeamMatch match in matches)
                {
                    RecordCount++;
                    if (string.IsNullOrEmpty(match.AirtableId))
                    {
                        Fields fields = new Fields();
                        fields.AddField("Uuid", match.Uuid.ToString());
                        fields.AddField("TeamNumber", match.TeamNumber);
                        fields.AddField("MatchNumber", match.MatchNumber);
                        fields.AddField("ScouterName", match.ScouterName);
                        fields.AddField("LeftTarmac", match.LeftTarmac);
                        fields.AddField("AutoHighGoals", match.AutoHighGoals);
                        fields.AddField("AutoLowGoals", match.AutoLowGoals);
                        fields.AddField("HumanHighGoals", match.HumanHighGoals);
                        fields.AddField("HumanLowGoals", match.HumanLowGoals);
                        fields.AddField("TeleHighGoals", match.TeleHighGoals);
                        fields.AddField("TeleLowGoals", match.TeleLowGoals);
                        fields.AddField("ClimbLevel", match.ClimbLevel);
                        fields.AddField("MatchRP", match.MatchRP);
                        fields.AddField("CargoRP", match.CargoRP);
                        fields.AddField("ClimbRP", match.ClimbRP);
                        fields.AddField("ScouterRating", match.ScouterRating);
                        fields.AddField("Comments", match.Comments);
                        newRecordList.Add(fields);
                    }
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
            }
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
                    // can only send 5 batches per second - make sure that doesn't happen
                    System.Threading.Thread.Sleep(500);
                }
            }
            return finalCount;
        }
    }
}
