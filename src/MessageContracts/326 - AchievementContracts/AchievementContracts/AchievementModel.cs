using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;
using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace CDMyAchievements.ViewModel
{
    public static class AchievementEngine
    {
        public const string Name = "CDMyAchievements.AchievmentService";
    }
    public class TheAchievementAward : TheDataBase
    {
        public string AchID { get; set; }
        public DateTimeOffset AwardTime { get; set; } = DateTimeOffset.Now;
        public double? Result { get; set; }

        public string InstanceName { get; set; }

        public Guid OwnerMID { get; set; }
        public TheAchievementAward()
        {

        }

        public TheAchievementAward(TT pThing)
        {
            OwnerMID = pThing.cdeMID;
        }
    }

    public class TheAchievementRegister : TheDataBase
    {
        public string AchievementAreaID { get; set; }
        public string Name { get; set; }
        public List<TheAchievement> Achievements { get; set; }
    }
    public class TheAchievement : TheDataBase
    {
        public int Progress { get; set; }
        public int ProgressStep { get; set; }
        public int ProgressGoal { get; set; }
        public string TaskToDo { get; set; }
        public string Name { get; set; }

        public Guid OwnerMID { get; set; }
        public bool IsBadAward { get; set; }
        public int Coins { get; set; }
        public string IconUrl { get; set; }
        public bool IsDisabled { get; set; }

        public string Medal { get; set; }
        public string MedalUrl { get; set; }

        public string AchID { get; set; }

        public string AchArea { get; set; }

        public bool AllowMultipleAwards { get; set; }

        public DateTimeOffset AwardAchievedAt { get; set; }
        public string AwardAchievedAtStr { get; set; }

        public double? AchievedResult { get; set; }

        public string ResultStr { get; set; }

        public string ResultFormat { get; set; }
        public double ResultScale { get; set; }

        public double? TriggerLevel { get; set; }

        public bool DontLog { get; set; }
        /// <summary>
        /// h=hour, d=day, w=week, m=month, y=year
        /// </summary>
        public string ResultReset { get; set; }
        public double MedalGold { get; set; }
        public double MedalSilver { get; set; }
        public double MedalBronce { get; set; }

        /// <summary>
        /// 0: always achieved 1:new Result must be higher 2:new result must be lower
        /// </summary>
        public int AchievedResultCompare { get; set; }

        public TheAchievement Clone()
        {
            var t = new TheAchievement();
            t.AchArea = AchArea;
            t.TaskToDo = TaskToDo;
            t.Name = Name;
            t.Coins = Coins;
            t.IconUrl = IconUrl;
            t.Medal = Medal;
            t.AchID = AchID;
            t.AllowMultipleAwards = AllowMultipleAwards;
            t.AchievedResult = AchievedResult;
            t.AchievedResultCompare = AchievedResultCompare;
            t.ResultScale = ResultScale;
            t.ResultStr = ResultStr;
            t.ResultFormat= ResultFormat;
            t.MedalGold = MedalGold;
            t.MedalSilver = MedalSilver;
            t.MedalBronce = MedalBronce;
            t.ResultReset = ResultReset;
            t.ProgressGoal=ProgressGoal;
            t.ProgressStep=ProgressStep;
            t.IsBadAward=IsBadAward;
            return t;
        }
    }

    public static class TheAchievementFactory
    {
        public static void InitAwards(TheAchievementRegister tAchi)
        {
            TheBaseEngine.WaitForEnginesStarted((sender, obj) =>
            {
                TheBaseEngine.WaitForEngineReady(AchievementEngine.Name, (sender2, obj2) =>
                {
                    TheCommCore.PublishCentral(new TSM(AchievementEngine.Name, "REGISTER_ACHIEVEMENTS", CU.SerializeObjectToJSONString(tAchi)), true);
                });
            });
        }

        public static void AwardAchievement(TheAchievementAward theAchievement)
        {
            TheCommCore.PublishCentral(new TSM(AchievementEngine.Name, "ACHI_REWARED", CU.SerializeObjectToJSONString(theAchievement)), true);
        }

        #region Time Summary Calc Calc
        private static readonly Dictionary<string, DateTimeOffset> MyLastMeasures = new Dictionary<string, DateTimeOffset>();
        public static void CalcTimeSummaries(TT pThing,string pInstanceName, string pAchIDSuffix, double tNewDelta, double ResultScale = 0.0, string ResultFormat = "", string SumSets="IHDMY", bool DeltaIsTotal=false)
        {
            if (ResultScale != 0.0)
                tNewDelta *= ResultScale;
            double tTotal = tNewDelta;
            if (!DeltaIsTotal)
            {
                tTotal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_Total", true));
                tTotal += tNewDelta;
                TT.SetSafePropertyNumber(pThing, $"{pAchIDSuffix}_Total", tTotal, true);
            }
            string ResultStr = CU.CStr(tTotal);
            if (!string.IsNullOrEmpty(ResultFormat))
                ResultStr = string.Format(ResultFormat, tTotal);
            pThing.SetProperty($"{pAchIDSuffix}_TotalStr", ResultStr);
            if (MyLastMeasures.TryGetValue($"{pAchIDSuffix}_{pThing.ID}", out var LastMeasure))
            {
                MyLastMeasures[$"{pAchIDSuffix}_{pThing.ID}"] = DateTimeOffset.Now;
                double tVal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_I", true));
                if (LastMeasure.Minute != DateTimeOffset.Now.Minute || tVal == 0.0)
                {
                    SetSummary(pThing, "I", pInstanceName, pAchIDSuffix, ResultFormat, SumSets, tTotal, tVal);
                }
                tVal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_H", true));
                if (LastMeasure.Hour != DateTimeOffset.Now.Hour || tVal == 0.0)
                {
                    SetSummary(pThing, "H", pInstanceName, pAchIDSuffix, ResultFormat, SumSets, tTotal, tVal);
                }
                tVal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_D", true));
                if (LastMeasure.Day != DateTimeOffset.Now.Day || tVal == 0.0)
                {
                    SetSummary(pThing, "D", pInstanceName, pAchIDSuffix, ResultFormat, SumSets, tTotal, tVal);
                }
                tVal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_M", true));
                if (LastMeasure.Month != DateTimeOffset.Now.Month || tVal == 0.0)
                {
                    SetSummary(pThing, "M", pInstanceName, pAchIDSuffix, ResultFormat, SumSets, tTotal, tVal);
                }
                tVal = CU.CDbl(pThing.GetProperty($"{pAchIDSuffix}_Y", true));
                if (LastMeasure.Year != DateTimeOffset.Now.Year || tVal == 0.0)
                {
                    SetSummary(pThing, "Y", pInstanceName, pAchIDSuffix, ResultFormat, SumSets, tTotal, tVal);
                }
            }
        }

        private static void SetSummary(TT pThing, string SumSetIdx, string pInstanceName, string pAchIDSuffix, string ResultFormat, string SumSets, double tTotal, double tVal)
        {
            var tResult = tTotal - tVal;
            if (tVal == 0.0)
                tResult = 0;
            else
            {
                if (SumSets?.Contains(SumSetIdx) == true)
                    AwardAchievement(new TheAchievementAward(pThing) { InstanceName = pInstanceName, Result = tResult, AchID = $"{pAchIDSuffix}{SumSetIdx}" });
            }
            TT.SetSafePropertyNumber(pThing, $"{pAchIDSuffix}_{SumSetIdx}", tTotal, true);
            TT.SetSafePropertyString(pThing, $"{pAchIDSuffix}_L{SumSetIdx}", GetResultString(0.0, ResultFormat, tTotal), true);
        }

        private static string GetResultString(double ResultScale, string ResultFormat, double tResult)
        {
            double tTempREs = tResult;
            if (ResultScale != 0.0)
                tTempREs *= ResultScale;
            var ResultStr = $"{tTempREs}";
            if (!string.IsNullOrEmpty(ResultFormat))
                ResultStr = string.Format(ResultFormat, tTempREs);
            return ResultStr;
        }

        public static Dictionary<string, TheFieldInfo> AddTimeSummaryNMI(TT pThing, TheFormInfo pForm, string pLabel, string pAchIDSuffix, string pUnit, int startFld, int parentFld)
        {
            NMI.AddSmartControl(pThing, pForm, eFieldType.TileGroup, startFld, 0, 0, null, null, new nmiCtrlTileGroup() { TileWidth = 6, ParentFld = parentFld });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 1, 0, 0, $"{pLabel} {pUnit} Total", $"{pAchIDSuffix}_TotalStr", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 2, 0, 0, $"{pLabel} {pUnit}/Min", $"{pAchIDSuffix}_LI", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 3, 0, 0, $"{pLabel} {pUnit}/Hour", $"{pAchIDSuffix}_LH", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 4, 0, 0, $"{pLabel} {pUnit}/Day", $"{pAchIDSuffix}_LD", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 5, 0, 0, $"{pLabel} {pUnit}/Month", $"{pAchIDSuffix}_LM", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });
            NMI.AddSmartControl(pThing, pForm, eFieldType.SingleEnded, startFld + 6, 0, 0, $"{pLabel} {pUnit}/Year", $"{pAchIDSuffix}_LY", new nmiCtrlSingleEnded { ParentFld = startFld, TileWidth = 2, NoTE = true });

            NMI.AddSmartControl(pThing, pForm, eFieldType.CollapsibleGroup, startFld + 7, 2, 0, "History Chart", null, new nmiCtrlCollapsibleGroup { ParentFld = startFld, TileWidth = 6, ClassName = "AXGroup", IsSmall = true });
            var Flds = ctrlC3Chart.AddC3Chart(pThing, pForm, startFld + 8, startFld + 7, new ctrlC3Chart { NoTE = true, TileHeight = 4, ChartType = "bar", TileWidth = 6, SetSeries = "[[\"No Data\", 0]]", UpdateData=true });
            Flds["ButI"]?.RegisterUXEvent(pThing, eUXEvents.OnClick, "ValueI", (sender, para) =>
            {
                RefreshChart(pThing, Flds["Chart"].cdeMID, $"{pAchIDSuffix}I");
            });
            Flds["ButH"]?.RegisterUXEvent(pThing, eUXEvents.OnClick, "ValueH", (sender, para) =>
            {
                RefreshChart(pThing, Flds["Chart"].cdeMID, $"{pAchIDSuffix}H");
            });
            Flds["ButD"]?.RegisterUXEvent(pThing, eUXEvents.OnClick, "ValueD", (sender, para) =>
            {
                RefreshChart(pThing, Flds["Chart"].cdeMID, $"{pAchIDSuffix}D");
            });
            Flds["ButM"]?.RegisterUXEvent(pThing, eUXEvents.OnClick, "ValueM", (sender, para) =>
            {
                RefreshChart(pThing, Flds["Chart"].cdeMID, $"{pAchIDSuffix}M");
            });
            Flds["ButY"]?.RegisterUXEvent(pThing, eUXEvents.OnClick, "ValueY", (sender, para) =>
            {
                RefreshChart(pThing, Flds["Chart"].cdeMID, $"{pAchIDSuffix}Y");
            });
            return Flds;
        }

        public static void RefreshChart(TT pThing, Guid FieldMID, string pAchIDSuffix)
        {
            var tAchi = new TheAchievement { AchID = pAchIDSuffix };
            TheCommCore.PublishCentral(new TSM(AchievementEngine.Name, $"GET_ACHIEVEMENTS:{FieldMID}:{pThing.cdeMID}", CU.SerializeObjectToJSONString(tAchi)), true);

        }

        public static void AchievementReceived(TT pThing, TSM pMsg)
        {
            if (pMsg?.TXT == null) return;
            string[] Command = pMsg.TXT.Split(':');
            switch (Command[0])
            {
                case "MY_ACHIEVEMENTS":
                    if (Command.Length > 2)
                    {
                        var tChart = NMI.GetFieldById(CU.CGuid(Command[1]));
                        var AchID = Command[2].Substring(Command[2].Length-1,1);
                        var tRes = CU.DeserializeJSONStringToObject<List<TheAchievement>>(pMsg.PLS);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        int i = 0;
                        foreach (var tA in tRes.OrderBy(s => s.AwardAchievedAt))
                        {
                            if (sb.Length > 2) sb.Append(",");
                            switch (AchID)
                            {
                                case "I":
                                    sb.Append($"[\"{tA.AwardAchievedAt.Minute}\", {tA.AchievedResult}]");
                                    break;
                                case "H":
                                    sb.Append($"[\"{tA.AwardAchievedAt.Hour}\", {tA.AchievedResult}]");
                                    break;
                                case "D":
                                    sb.Append($"[\"{tA.AwardAchievedAt.Day}\", {tA.AchievedResult}]");
                                    break;
                                case "M":
                                    sb.Append($"[\"{tA.AwardAchievedAt.Month}\", {tA.AchievedResult}]");
                                    break;
                                case "Y":
                                    sb.Append($"[\"{tA.AwardAchievedAt.Year}\", {tA.AchievedResult}]");
                                    break;
                            }
                            i++; if (i > 11) break;
                        }
                        sb.Append("]");
                        if (sb.Length == 2)
                            sb = new StringBuilder("[[\"No Data\", 0]]");
                        tChart.SetUXProperty(Guid.Empty, $"DataModel={sb}");
                    }
                    break;
            }
        }

        public static List<TheAchievement> AddSummaryAchievements(string pThingID, string pAchIDSuffix, string NameSuffix, string Todo,bool bDontLogIH,bool bDontLogDMY, double ResultScale = 0.0, string pFormat = null)
        {
            var Achievements = new List<TheAchievement>
                {
                    new TheAchievement { AchID = $"{pAchIDSuffix}I", AchievedResultCompare = 0, AllowMultipleAwards=true, Name = $"{NameSuffix} last Minute", TaskToDo = Todo, DontLog=bDontLogIH, ResultScale = ResultScale, ResultFormat = pFormat, ResultReset = "i" },
                    new TheAchievement { AchID = $"{pAchIDSuffix}H", AchievedResultCompare = 0, AllowMultipleAwards=true, Name = $"{NameSuffix} last Hour", TaskToDo = Todo, DontLog=bDontLogIH, ResultScale = ResultScale, ResultFormat = pFormat, ResultReset = "h" },
                    new TheAchievement { AchID = $"{pAchIDSuffix}D", AchievedResultCompare = 0, AllowMultipleAwards=true, Name = $"{NameSuffix} last Day", TaskToDo = Todo, DontLog=bDontLogDMY,ResultScale = ResultScale, ResultFormat = pFormat, ResultReset = "d" },
                    new TheAchievement { AchID = $"{pAchIDSuffix}M", AchievedResultCompare = 0, AllowMultipleAwards=true, Name = $"{NameSuffix} last Month", TaskToDo = Todo, DontLog=bDontLogDMY, ResultScale = ResultScale, ResultFormat = pFormat, ResultReset = "m" },
                    new TheAchievement { AchID = $"{pAchIDSuffix}Y", AchievedResultCompare = 0, AllowMultipleAwards=true, Name = $"{NameSuffix} last Year", TaskToDo = Todo, DontLog=bDontLogDMY, ResultScale = ResultScale, ResultFormat = pFormat, ResultReset = "y" }
                };
            MyLastMeasures[$"{pAchIDSuffix}_{pThingID}"] = DateTimeOffset.Now;

            return Achievements;
        }
        #endregion
    }

    public partial class ctrlC3Chart : TheNMIBaseControl
    {
        /// <summary> 
        /// Default Value: transparent
        /// </summary>
        public string Background { get; set; }

        public string SetSeries { get; set; }

        public string ChartColors { get; set; }

        public string ChartType { get; set; }

        public string Groups { get; set; }

        public bool UpdateData { get; set; }

        public double MaxValue { get; set; }


        public override string EngineName => "CDMyC3.TheC3Service";

        public override string ControlType => "CDMyC3.ctrlC3Chart";

        public static Dictionary<string,TheFieldInfo> AddC3Chart(TT pMyBaseThing, TheFormInfo pForm, int pFldOrder, int parentFld, ThePropertyBag pBag)
        {
            if (pBag == null)
                pBag = new ThePropertyBag();
            if (parentFld >= 0)
                ThePropertyBag.PropBagUpdateValue(pBag, "ParentFld", "=", parentFld.ToString());
            Dictionary<string, TheFieldInfo> Flds = new Dictionary<string, TheFieldInfo>();
            var tChartCtrl = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.UserControl, pFldOrder, 2, 0, null, "SampleProperty", pBag);
            Flds["Chart"] = tChartCtrl;
            Flds["ButI"] = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.TileButton, pFldOrder + 2, 2, 0, "Min", null, new nmiCtrlTileButton { TileWidth = 1, TileFactorY=2, NoTE = true, ParentFld = parentFld, ClassName = "cdeGoodActionButton" });
            Flds["ButH"] = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.TileButton, pFldOrder + 3, 2, 0, "Hour", null, new nmiCtrlTileButton { TileWidth = 1, TileFactorY = 2, NoTE = true, ParentFld = parentFld, ClassName = "cdeGoodActionButton" });
            Flds["ButD"] = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.TileButton, pFldOrder + 4, 2, 0, "Day", null, new nmiCtrlTileButton { TileWidth = 1, TileFactorY = 2, NoTE = true, ParentFld = parentFld, ClassName = "cdeGoodActionButton" });
            Flds["ButM"] = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.TileButton, pFldOrder + 5, 2, 0, "Month", null, new nmiCtrlTileButton { TileWidth = 1, TileFactorY = 2, NoTE = true, ParentFld = parentFld, ClassName = "cdeGoodActionButton" });
            Flds["ButY"] = NMI.AddSmartControl(pMyBaseThing, pForm, eFieldType.TileButton, pFldOrder + 6, 2, 0, "Year", null, new nmiCtrlTileButton { TileWidth = 1, TileFactorY = 2, NoTE = true, ParentFld = parentFld, ClassName = "cdeGoodActionButton" });
            return Flds;
        }
    }
}
