/*
Copyright (c) <2015>, <CSTB>
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FMICsharp
{
     [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface  IFMI2
    {
                

        #region Fonctions Communes

        /***************************************************
        Common Functions
        ****************************************************/

        /* Inquire version numbers of header files and setting logging status */
        fmi2Status fmi2SetDebugLogging(bool loggingOn, int nCategories, string[] categories);

        IFMI2 fmi2Instantiate(string instanciateName, int fmuType, string fmuGUID, string fmuResourceLocation, bool visible, bool loggingOn);        

        void fmi2FreeInstance();

        fmi2Status fmi2SetupExperiment(bool toleranceDefined, double tolerance, double startTime, bool stopTimeDefined, double stopTime);

        fmi2Status fmi2EnterInitializationMode();

        fmi2Status fmi2ExitInitializationMode();
        fmi2Status fmi2Terminate();
        fmi2Status fmi2Reset();

        /* Getting and setting variable values */
        fmi2Status fmi2GetReal(int[] vr,ref double[] value);
        fmi2Status fmi2GetInteger(int[] vr,ref int[] value);
        fmi2Status fmi2GetBoolean(int[] vr,ref bool[] value);
        fmi2Status fmi2GetString(int[] vr,ref String[] value);
        fmi2Status fmi2SetReal(int[] vr, double[] value);
        fmi2Status fmi2SetInteger(int[] vr, int[] value);
        fmi2Status fmi2SetBoolean(int[] vr, bool[] value);
        fmi2Status fmi2SetString(int[] vr,string[] value);


        /* Getting and setting the internal FMU state */
        fmi2Status fmi2GetFMUstate(ref fmi2Status FMUstate);
        fmi2Status fmi2SetFMUstate(fmi2Status FMUstate);
        fmi2Status fmi2FreeFMUstate(fmi2Status FMUstate);
        fmi2Status fmi2SerializedFMUstateSize(fmi2Status FMUstate,ref int size);
        fmi2Status fmi2SerializeFMUstate(fmi2Status  FMUstate,ref byte[] serializeState);
        fmi2Status fmi2DeSerializeFMUstate(ref byte[] serializeState,  fmi2Status FMUstate);

        /* Getting partial derivatives */
        fmi2Status fmi2GetDirectionalDerivative(ref int[] vrUnknown_ref,ref int[] vKnown,ref double[] dvKnown,ref double[] dvUnknown);

        

#endregion Fonctions communes

/***************************************************
Types for Functions for FMI2 for Model Exchange
****************************************************/

/* Enter and exit the different modes */
fmi2Status fmi2EnterEventMode();
fmi2Status fmi2NewDiscreteStates(ref  fmi2EventInfo fmi2eventInfo);
fmi2Status fmi2EnterContinuousTimeMode();
fmi2Status fmi2CompletedIntegratorStep( bool noSetFMUStatePriorToCurrentPoint,ref bool enterEventMode,ref bool terminateSimulation);

/* Providing independent variables and re-initialization of caching */
fmi2Status fmi2SetTime(double time);
fmi2Status fmi2SetContinuousStates(double[] x);

/* Evaluation of the model equations */
fmi2Status fmi2GetDerivatives(ref double[] derivatives);
fmi2Status fmi2GetEventIndicators(ref double[] eventIndicators);
fmi2Status fmi2GetContinuousStates(ref double[] x);
fmi2Status fmi2GetNominalsOfContinuousStates(ref double[] x_nominal);


///***************************************************
//Functions for FMI2 for Co-Simulation
//****************************************************/

/* Simulating the slave */
fmi2Status fmi2SetRealInputDerivatives(int[] vr, int[] order, double[] value);
fmi2Status fmi2GetRealOutputDerivatives(int[] vr, int[] order,ref double[] value);

fmi2Status fmi2DoStep(
    double currentCommunicationPoint,
    double communicationStepSize,
    bool noSetFMUStatePriorToCurrentPoint);

fmi2Status fmi2CancelStep();

/* Inquire slave status */
fmi2Status fmi2GetStatus(fmi2StatusKind s, ref  int value);
fmi2Status fmi2GetRealStatus(fmi2StatusKind s,ref double value);
fmi2Status fmi2GetIntegerStatus(fmi2StatusKind s,ref int value);
fmi2Status fmi2GetBooleanStatus(fmi2StatusKind s, ref bool value);
fmi2Status fmi2GetStringStatus( fmi2StatusKind s,ref string  value);




    }
}
