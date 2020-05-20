// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !NETSTANDARD2_0 && !CDE_NEWPROJECTSYSTEM

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("cdeCryptoLib")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("cdeCryptoLib")]
[assembly: AssemblyCopyright("Copyright © 2018-2020 TRUMPF Laser GmbH, authors: C-Labs")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a8dabdc1-af7f-4fdb-a6f9-b3f196952fbf")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.212.*")]
[assembly: AssemblyFileVersion("4.212.0.0")]
#endif
[assembly: InternalsVisibleTo("C-DEngine (PC Net 4.5).Tests")]
[assembly: InternalsVisibleTo("cdeActivation")]
[assembly: InternalsVisibleTo("cdeLicenseTool")]
