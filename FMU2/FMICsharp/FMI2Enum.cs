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
    public enum fmi2Status : int
    {
        fmi2OK = 0,
        fmi2Warning = 1,
        fmi2Discard = 2,
        fmi2Error = 3,
        fmi2Fatal = 4,
        fmi2Pending = 5
    };

    public enum fmi2Type : int
    {
        fmi2ModelExchange = 0,
        fmi2CoSimulation = 1
    }

    public enum fmi2StatusKind : int
    {
        fmi2DoStepStatus = 0,
        fmi2PendingStatus = 1,
        fmi2LastSuccessfulTime = 2,
        fmi2Terminated = 3
    }
    public enum fmi2CategorieQtronic : int
    {
        LOG_ALL = 0,
        LOG_ERROR = 1,
        LOG_FMI_CALL = 2,
        LOG_EVENT = 3,
    }
    public enum fmi2ModelState :int
    {
        modelStartAndEnd = 1 << 0,
        modelInstantiated = 1 << 1,
        modelInitializationMode = 1 << 2,

        // ME states
        modelEventMode = 1 << 3,
        modelContinuousTimeMode = 1 << 4,
        // CS states
        modelStepComplete = 1 << 5,
        modelStepInProgress = 1 << 6,
        modelStepFailed = 1 << 7,
        modelStepCanceled = 1 << 8,

        modelTerminated = 1 << 9,
        modelError = 1 << 10,
        modelFatal = 1 << 11,
    }



  
}