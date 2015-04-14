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
using System.Text;
using System.Threading.Tasks;

namespace FMICsharp
{
    class Mask
    {

        // ---------------------------------------------------------------------------
        // Function calls allowed state masks for both Model-exchange and Co-simulation
        // ---------------------------------------------------------------------------
        public static fmi2ModelState MASK_fmi2GetTypesPlatform = (fmi2ModelState.modelStartAndEnd | fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepInProgress | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2GetVersion = MASK_fmi2GetTypesPlatform;

        public static fmi2ModelState MASK_fmi2SetDebugLogging = (fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepInProgress | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2Instantiate = (fmi2ModelState.modelStartAndEnd);
        public static fmi2ModelState MASK_fmi2FreeInstance = (fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2SetupExperiment = fmi2ModelState.modelInstantiated;
        public static fmi2ModelState MASK_fmi2EnterInitializationMode = fmi2ModelState.modelInstantiated;
        public static fmi2ModelState MASK_fmi2ExitInitializationMode = fmi2ModelState.modelInitializationMode;
        public static fmi2ModelState MASK_fmi2Terminate = (fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepFailed);
        public static fmi2ModelState MASK_fmi2Reset = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2GetReal = (fmi2ModelState.modelInitializationMode | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2GetInteger = MASK_fmi2GetReal;
        public static fmi2ModelState MASK_fmi2GetBoolean = MASK_fmi2GetReal;
        public static fmi2ModelState MASK_fmi2GetString = MASK_fmi2GetReal;
        public static fmi2ModelState MASK_fmi2SetReal = (fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode | fmi2ModelState.modelStepComplete);
        public static fmi2ModelState MASK_fmi2SetInteger = (fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode | fmi2ModelState.modelEventMode | fmi2ModelState.modelStepComplete);
        public static fmi2ModelState MASK_fmi2SetBoolean = MASK_fmi2SetInteger;
        public static fmi2ModelState MASK_fmi2SetString = MASK_fmi2SetInteger;
        public static fmi2ModelState MASK_fmi2GetFMUstate = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2SetFMUstate = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2FreeFMUstate = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2SerializedFMUstateSize = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2SerializeFMUstate = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2DeSerializeFMUstate = MASK_fmi2FreeInstance;
        public static fmi2ModelState MASK_fmi2GetDirectionalDerivative = (fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);

        // ---------------------------------------------------------------------------
        // Function calls allowed state masks for Model-exchange
        // ---------------------------------------------------------------------------
        public static fmi2ModelState MASK_fmi2EnterEventMode = (fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode);
        public static fmi2ModelState MASK_fmi2NewDiscreteStates = fmi2ModelState.modelEventMode;
        public static fmi2ModelState MASK_fmi2EnterContinuousTimeMode = fmi2ModelState.modelEventMode;
        public static fmi2ModelState MASK_fmi2CompletedIntegratorStep = fmi2ModelState.modelContinuousTimeMode;
        public static fmi2ModelState MASK_fmi2SetTime = (fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode);
        public static fmi2ModelState MASK_fmi2SetContinuousStates = fmi2ModelState.modelContinuousTimeMode;
        public static fmi2ModelState MASK_fmi2GetEventIndicators = (fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2GetContinuousStates = MASK_fmi2GetEventIndicators;
        public static fmi2ModelState MASK_fmi2GetDerivatives = (fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2GetNominalsOfContinuousStates = (fmi2ModelState.modelInstantiated
                                                | fmi2ModelState.modelEventMode | fmi2ModelState.modelContinuousTimeMode
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);

        // ---------------------------------------------------------------------------
        // Function calls allowed state masks for Co-simulation
        // ---------------------------------------------------------------------------
        public static fmi2ModelState MASK_fmi2SetRealInputDerivatives = (fmi2ModelState.modelInstantiated | fmi2ModelState.modelInitializationMode
                                                | fmi2ModelState.modelStepComplete);
        public static fmi2ModelState MASK_fmi2GetRealOutputDerivatives = (fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepFailed | fmi2ModelState.modelStepCanceled
                                                | fmi2ModelState.modelTerminated | fmi2ModelState.modelError);
        public static fmi2ModelState MASK_fmi2DoStep = fmi2ModelState.modelStepComplete;
        public static fmi2ModelState MASK_fmi2CancelStep = fmi2ModelState.modelStepInProgress;
        public static fmi2ModelState MASK_fmi2GetStatus = (fmi2ModelState.modelStepComplete | fmi2ModelState.modelStepInProgress | fmi2ModelState.modelStepFailed
                                                | fmi2ModelState.modelTerminated);
        public static fmi2ModelState MASK_fmi2GetRealStatus = MASK_fmi2GetStatus;
        public static fmi2ModelState MASK_fmi2GetIntegerStatus = MASK_fmi2GetStatus;
        public static fmi2ModelState MASK_fmi2GetBooleanStatus = MASK_fmi2GetStatus;
        public static fmi2ModelState MASK_fmi2GetStringStatus = MASK_fmi2GetStatus;
    }
}
