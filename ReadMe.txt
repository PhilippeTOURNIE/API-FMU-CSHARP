************************************************************************************************
*                                     Project FMU c#                                            *
*                                                                                               *
*************************************************************************************************
*                         le 14/04/2015  Created by Philippe TOURNIE
*************************************************************************************************

the goal is to create and Run a FMU c# in a Simulation environment using official FMI2 standard. 

This Repository provide :
 - a FMI C# SDK 
 - a Wrapper betwwen FMU Sdk and FMUc#
 - a Sample FMU c#  call "Value" from QTronic FMU sdk

*********************
Environment:
*********************
Running on windows 
Visual studio 2013  C++ c# 
Framework.net  4.5.1 


************************************
Treview of the solution's Project :
************************************
|
|
------ FMICsharp  (FMI C# sdk)
|
------ FMU2ManageCsharp (Wrapper Api com for interop c#<=>c++)
|
------ FMUCTemplate (wrapper FMU c++ <=> Api )
|
------ FMUCsharp ( a sample "Value" from QTronic FMU sdk )  


**************
Process
**************
Step by step :
1) Simulation call a FMUCTemplate c++  
2) FMUCTemplate call API COM FMU2ManageCsharp 
3) FMU2ManageCsharp call FMUCsharp
4) FMUCsharp run and return response to the API FMU2ManageCsharp
5) the API FMU2ManageCsharp return info to the a FMUCTemplate c++  
6) finally the FMUCTemplate c++ send info to the simulation

 
****************
Sample 'Value'
****************
Sample/value_cs/Value.FMU  (co-simulation)
Sample/value_me/Value.FMU  (model exchange)

Value.FMU 
|
---- binaries
	|
	--- win32 
		|
		----- values.dll ( FMUCTemplate c++)
|
----Resources
	|
	---- FMUCsharp.dll (FMUCsharp sample "value")
	| 
	---- FMICsharp.dll (Lib sdk c#)

*****************
Installation 
*****************

For Test
1) Install FMU sdk Qtronic
https://www.qtronic.de/en/fmusdk.html


2) register API COM FMU2ManageCsharp  on your PC.
Command Line :
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe  FMUManageCsharp.dll /codebase /tlb:FMUManageCsharp.tlb

************************
Create Your own FMU C#
************************

Not automation for the moment, we need to create a new project FMU_"Model" inside the solution with modeldescription.xml.
Your model must implement the abstract class "AbstractFMU". 
There is no generation of modelDescription xml yet...

[The original idea is to generate the file "FMUMode.cpp" of the project FMUCTemplate with parameters
you define in your FMU (Name and GUID).]

You have to modify "FMUMode.cpp"  of the project FMUCTemplate, to set the name and the guid :

// Name of the model
#define MODEL_IDENTIFIER values
// Guid of the model
#define MODEL_GUID "{8c4e810f-3df3-4a00-8276-176fa3c9f004}"

Next, compile your FMU Modele and FMUCTemplate and Copy output dll in appropriate folder of your FMU'sfolder.
 /resource  for FMU c#  and bin/ for FMUCTemplate (according your cpu 32 or 64) .


***********************
Test your FMU
*********************** 
I use Qtronic Batch replace the 'values' by the name of your FMU. Select if you want to run in CS or ME mode !!
see file below :

----------------------------------------------------------------
@echo off 

rem ------------------------------------------------------------
rem This batch runs all FMUs of the FmuSDK and stores simulation
rem results in CSV files, one file per simulation run.
rem Command to run the 32 bit version: run_all
rem Command to run the 64 bit version: run_all -win64
rem Copyright QTronic GmbH. All rights reserved.
rem ------------------------------------------------------------

setlocal
if "%1"=="-win64" (
set x64=x64\
set x64NAME=x64
) else (
set x64=
set x64NAME=
)

echo -----------------------------------------------------------
call fmusim me20 fmu20\fmu\me\%x64%values.fmu 12 0.1 0 c %1
move /Y result.csv result_me20%x64NAME%_values.csv

echo -----------------------------------------------------------
call fmusim cs20 fmu20\fmu\cs\%x64%values.fmu 12 0.1 0 c %1
move /Y result.csv result_cs20%x64NAME%_values.csv

endlocal

rem keep window visible for user
pause
------------------------------------------------------------------



************************
Tasks improvement
************************
- Generate modelDescription.xml 
- in API COM unload FMU assembly when simulation ended 
- make a script to change FMUCTemplate (Name and GUID of the FMU).
- Script to zip FMU....
- avaliable for one instance FMU 








