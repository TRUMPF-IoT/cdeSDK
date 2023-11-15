// SPDX-FileCopyrightText: 2009-2023 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0
using nsCDEngine.Engines.ThingService;
using System;
using System.Collections.Generic;
using System.Text;
using CU = nsCDEngine.BaseClasses.TheCommonUtils;

namespace cdeEnergyBase
{
    #region UA/PCM NodeSet Base Classes
    public enum AccuracyDomainEnum
    {
        ACCURACY_DOMAIN_RESERVED = 0,
        ACCURACY_DOMAIN_PERCENT_FULL_SCALE = 1,
        ACCURACY_DOMAIN_PERCENT_ACTUAL_READING = 2,
        ACCURACY_DOMAIN_IEC = 3,
        ACCURACY_DOMAIN_EN = 4,
    }
    public enum AccuracyClassEnum
    {
        ACCURACY_CLASS_0 = 0,
        ACCURACY_CLASS_1 = 1,
        ACCURACY_CLASS_2 = 2,
        ACCURACY_CLASS_3 = 3,
        ACCURACY_CLASS_4 = 4,
        ACCURACY_CLASS_5 = 5,
        ACCURACY_CLASS_6 = 6,
        ACCURACY_CLASS_7 = 7,
        ACCURACY_CLASS_8 = 8,
        ACCURACY_CLASS_9 = 9,
        ACCURACY_CLASS_10 = 10,
        ACCURACY_CLASS_11 = 11,
        ACCURACY_CLASS_12 = 12,
        ACCURACY_CLASS_13 = 13,
        ACCURACY_CLASS_14 = 14,
        ACCURACY_CLASS_15 = 15,
    }

    [OPCUAType(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=2002")]
    public class EnergyMeasurementValueType : UABaseVariableType
    {
        public EnergyMeasurementValueType(ICDEProperty pBaseProperty, string pCDEPName) : base(pBaseProperty, pCDEPName) { }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6020", UAMandatory = true)]
        public AccuracyClassEnum AccuracyClass
        {
            get { return (AccuracyClassEnum)CU.CInt(MemberGetProperty()); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6019", UAMandatory = true)]
        public AccuracyDomainEnum AccuracyDomain
        {
            get { return (AccuracyDomainEnum)CU.CInt(MemberGetProperty()); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6010", UAMandatory = true)]
        public object AccuracyRange
        {
            get { return MemberGetProperty(); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6021")]
        public string EngineeringUnits
        {
            get { return CU.CStr(MemberGetProperty()); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6018", UAMandatory = true)]
        public uint MeasurementID
        {
            get { return CU.CUInt(MemberGetProperty()); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6015")]
        public int MeasurementPeriod
        {
            get { return CU.CInt(MemberGetProperty()); }
            set { MemberSetProperty(value); }
        }

        [OPCUAVariable(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6017", UAMandatory = true)]
        public object Resource
        {
            get { return MemberGetProperty(); }
            set { MemberSetProperty(value); }
        }

        [OPCUAProperty(UATypeNodeId = "nsu=http://opcfoundation.org/UA/PCM/;i=6022")]
        public object ValueBeforeReset
        {
            get { return MemberGetProperty(); }
            set { MemberSetProperty(value); }
        }
    }
    #endregion
}
