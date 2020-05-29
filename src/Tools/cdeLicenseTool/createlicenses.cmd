rem SPDX-FileCopyrightText: Copyright (c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
rem SPDX-License-Identifier: CC0-1.0
set licensetoolpath=%~1
if "%licensetoolpath%"=="" set licensetoolpath=.\bin\debug\cdelicensetool.exe
set snkfile=%~2
if "%snkfile%"=="" set snkfile=.\Keys\sdkdemo.snk 
call %licensetoolpath% createlicense .\Templates\ %snkfile% .\Licenses\
