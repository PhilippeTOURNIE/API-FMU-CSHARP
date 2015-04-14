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
Description file 
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

Not automation for the moment, we need to create a new project FMU_"Model" inside the solution 

not finish 
TODO



The original idea is to generate the file "FMUMode.cpp" of the project FMUCTemplate

// Nom du modele ici pour le test c'est value
// en DLL Share il ne faut pas le définir #define FMI2_FUNCTION_PREFIX MyModel_ values
#define MODEL_IDENTIFIER values
// Guid xml et dll doivent correspondre
#define MODEL_GUID "{8c4e810f-3df3-4a00-8276-176fa3c9f004}"











