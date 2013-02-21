@echo off
rem  ----------------------------------------------------------------------------
rem          ATMEL Microcontroller Software Support  -  ROUSSET  -
rem  ----------------------------------------------------------------------------
rem  Copyright (c) 2007, Atmel Corporation
rem
rem  All rights reserved.
rem
rem  Redistribution and use in source and binary forms, with or without
rem  modification, are permitted provided that the following conditions are met:
rem
rem  - Redistributions of source code must retain the above copyright notice,
rem  this list of conditions and the disclaiimer below.
rem
rem  - Redistributions in binary form must reproduce the above copyright notice,
rem  this list of conditions and the disclaimer below in the documentation and/or
rem  other materials provided with the distribution. 
rem
rem  Atmel's name may not be used to endorse or promote products derived from
rem  this software without specific prior written permission. 
rem
rem  DISCLAIMER: THIS SOFTWARE IS PROVIDED BY ATMEL "AS IS" AND ANY EXPRESS OR
rem  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
rem  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT ARE
rem  DISCLAIMED. IN NO EVENT SHALL ATMEL BE LIABLE FOR ANY DIRECT, INDIRECT,
rem  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
rem  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
rem  OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
rem  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
rem  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
rem  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
rem  ----------------------------------------------------------------------------

rem ###############################################################################
rem  FLASH programming script file exemple
rem ###############################################################################

cls
echo ###############################################################################
echo This task will take about 3 minutes so please be patient and do not interrupt it
echo.
echo A log file will be open when this is all complete...
echo.
echo To terminate this shell you can *CLOSE* the log file (after it is opened in notepad!)
echo ###############################################################################
@echo off

SAM-BA_cdc %1 at91sam9rl64-ek TinyBooterLoader.tcl > logfile.log 2>&1
echo 
notepad logfile.log
