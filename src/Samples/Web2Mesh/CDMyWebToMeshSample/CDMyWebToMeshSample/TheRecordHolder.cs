// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyWebToMeshSample
{
    public class TheRecordHolder
    {
        public int id;
        public string strTime;
        public string strName;
        public string strNationality;
        public string strDate;


        public TheRecordHolder(int i, string t, string name, string nat, string d)
        {
            id = i;
            strTime = t;
            strName = name;
            strNationality = nat;
            strDate = d;
        }

        public static TheRecordHolder[] aData = new TheRecordHolder[]
        {
            new TheRecordHolder( 1, "03:43.1", "Hicham El Guerrouj", "Morocco", "7 July 1999" ),
            new TheRecordHolder( 2, "03:44.4", "Noureddine Morceli", "Algeria", "5 September 1993" ),
            new TheRecordHolder( 3, "03:46.3", "Steve Cram", "United Kingdom", "27 July 1985" ),
            new TheRecordHolder( 4, "03:47.3", "Sebastian Coe", "United Kingdom", "28 August 1981" ),
            new TheRecordHolder( 5, "03:48.4", "Steve Ovett", "United Kingdom", "26 August 1981" ),
        };

        public static TheRecordHolder QueryRecordHolder(int id)
        {
            if (id > 0 && id <= TheRecordHolder.aData.Length)
                return TheRecordHolder.aData[id - 1];
            else
                return null;
        }

    } // class
} // namespace
