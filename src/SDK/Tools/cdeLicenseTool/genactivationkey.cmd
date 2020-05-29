rem SPDX-FileCopyrightText: Copyright (c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
rem SPDX-License-Identifier: CC0-1.0
rem %1 = deviceid, %2 = SigningKey
bin\debug\net45\cdeLicenseTool.exe generateactivationkey %1 %1 0 .\Keys\test.pubbytes .\licensecontainers
