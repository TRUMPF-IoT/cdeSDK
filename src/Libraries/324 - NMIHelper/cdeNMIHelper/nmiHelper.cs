// SPDX-FileCopyrightText: 2009-2021 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

using NMI = nsCDEngine.Engines.NMIService.TheNMIEngine;
using TT = nsCDEngine.Engines.ThingService.TheThing;

namespace cdeNMIHelper
{
    public static class nmiHelper
    {
        public static TheFieldInfo AddLineEdit(TheThing MyBaseThing, TheFormInfo pForm, eFieldType pType, int StartFld, int ParentFld, int flg, string pLabel, string pUpdateName, int Width, ThePropertyBag pBag = null)
        {
            NMI.AddSmartControl(MyBaseThing, pForm, eFieldType.TileGroup, StartFld, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = ParentFld, TileWidth = Width, TileHeight = 1 });
            NMI.AddSmartControl(MyBaseThing, pForm, eFieldType.SmartLabel, StartFld + 1, 0, 0, null, null, new nmiCtrlSmartLabel { Style = "width:80%; font-size:16px; text-align:left", TileFactorY = 4, NoTE = true, Text = pLabel, ParentFld = StartFld, TileWidth = Width });
            var fld = NMI.AddSmartControl(MyBaseThing, pForm, pType, StartFld + 2, flg, 0, null, pUpdateName, new nmiCtrlNumber { NoTE = true, ParentFld = StartFld, TileFactorY = 2, TileWidth = Width });
            if (pBag != null)
                fld.PropertyBag = pBag;
            return fld;
        }

        public static TheFieldInfo Add4x2Edit(TheThing MyBaseThing, TheFormInfo pForm, eFieldType pType, int StartFld, int ParentFld, int flg, string pLabel, string pUpdateName, ThePropertyBag pBag = null)
        {
            NMI.AddSmartControl(MyBaseThing, pForm, eFieldType.TileGroup, StartFld, 0, 0, null, null, new nmiCtrlTileGroup { ParentFld = ParentFld, TileWidth = 6 });
            NMI.AddSmartControl(MyBaseThing, pForm, eFieldType.SmartLabel, StartFld + 1, 0, 0, null, null, new nmiCtrlSmartLabel { Style = "width:80%; font-size:20px; text-align:left; padding:20px;", NoTE = true, Text = pLabel, ParentFld = StartFld, TileWidth = 4 });
            var fld = NMI.AddSmartControl(MyBaseThing, pForm, pType, StartFld + 2, flg, 0, null, pUpdateName, new nmiCtrlNumber { ParentFld = StartFld, TileWidth = 2 });
            if (pBag != null)
                fld.PropertyBag = pBag;
            return fld;
        }

        public static TheFieldInfo AddSpeedGauge(TheThing MyBaseThing, TheFormInfo tMyForm, int pFldNumber, int pParentFld, string Label, string Units, string UpdateName, int MinVal, int MaxVal, int errLevel, int Wid = 4, bool invertRange = false)
        {
            string Plotband = $"PlotBand=[{{ \"from\": {MinVal}, \"to\": {errLevel}, \"color\": \"#FF000088\" }}, {{ \"from\": {errLevel}, \"to\": {MaxVal}, \"color\": \"#00FF0044\" }}]";
            if (invertRange)
                Plotband = $"PlotBand=[{{ \"from\": {MinVal}, \"to\": {errLevel}, \"color\": \"#00FF0044\" }}, {{ \"from\": {errLevel}, \"to\": {MaxVal}, \"color\": \"#FF000088\" }}]";

            return NMI.AddSmartControl(MyBaseThing, tMyForm, eFieldType.UserControl, pFldNumber, 0, 0, null, UpdateName, new ThePropertyBag() { "ControlType=Speed Gauge", "NoTE=true",$"ParentFld={pParentFld}",
                    $"TileWidth={Wid}",$"TileHeight={Wid}",
                    $"MinValue={MinVal}",
                    $"MaxValue={MaxVal}",
                    $"SubTitle={Units}",
                    Plotband,
                    $"SetSeries={{ \"name\": \"{Label}\",\"data\": [{TheThing.GetSafePropertyNumber(MyBaseThing,UpdateName)}],\"tooltip\": {{ \"valueSuffix\": \" {Units}\"}}}}",
                $"Value={TT.GetSafePropertyNumber(MyBaseThing,UpdateName)}"
                     });
        }

        public static TheFieldInfo UpdateSpeedGauge(TheFieldInfo pInfo, int MinVal, int MaxVal, int errLevel, bool invertRange = false)
        {
            if (pInfo == null)
                return null;
            string Plotband = $"PlotBand=[{{ \"from\": {MinVal}, \"to\": {errLevel}, \"color\": \"#FF000088\" }}, {{ \"from\": {errLevel}, \"to\": {MaxVal}, \"color\": \"#00FF0044\" }}]";
            if (invertRange)
                Plotband = $"PlotBand=[{{ \"from\": {MinVal}, \"to\": {errLevel}, \"color\": \"#00FF0044\" }}, {{ \"from\": {errLevel}, \"to\": {MaxVal}, \"color\": \"#FF000088\" }}]";

            pInfo.PropertyBag = new ThePropertyBag() {
                    $"MinValue={MinVal}",
                    $"MaxValue={MaxVal}",
                    Plotband
                     };
            return pInfo;
        }
    }
}
