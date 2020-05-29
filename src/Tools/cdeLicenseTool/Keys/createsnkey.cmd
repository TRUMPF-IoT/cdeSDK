rem SPDX-FileCopyrightText: Copyright (c) 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
rem SPDX-License-Identifier: CC0-1.0
@echo off
if exist %1.snk goto error
sn -k %1.snk
sn -o %1.snk %1.pub
goto exit
:error
echo %1.snk already exists.
:exit
