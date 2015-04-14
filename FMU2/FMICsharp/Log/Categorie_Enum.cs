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

namespace FMICsharp.Log
{
    /*Category name Description
logEvents Log all events (during initialization and simulation).
logSingularLinearSystems Log the solution of linear systems of equations if the solution is singular
(and the tool picked one solution of the infinitely many solutions).
logNonlinearSystems Log the solution of nonlinear systems of equations.
logDynamicStateSelection Log the dynamic selection of states.
logStatusWarning Log messages when returning fmi2Warning status from any function.
logStatusDiscard Log messages when returning fmi2Discard status from any function.
logStatusError Log messages when returning fmi2Error status from any function.
logStatusFatal Log messages when returning fmi2Fatal status from any function.
logStatusPending Log messages when returning fmi2Pending status from any function.
logAll Log all messages.*/

    public enum CategorieNameEnum : int
    {
        logEvents = 0,
        logSingularLinearSystems = 1,
        logNonlinearSystems = 2,
        logDynamicStateSelection = 3,
        logStatusWarning = 4,
        logStatusDiscard = 5,
        logStatusError = 6,
        logStatusFatal = 7,
        logStatusPending = 8,
        logAll = 9,
    }
    public static class FMI_Categorie
    {
        public static  string[] DescriptionCategories = new string[]
        {
            "Log all events (during initialization and simulation)",
            "Log the solution of linear systems of equations if the solution is singular (and the tool picked one solution of the infinitely many solutions).",
            "Log the solution of nonlinear systems of equations.",
            "Log the dynamic selection of states.",
            "Log messages when returning fmi2Warning status from any function.",
            "Log messages when returning fmi2Discard status from any function.",
            "Log messages when returning fmi2Error status from any function.",
            "Log messages when returning fmi2Fatal status from any function.",
            "Log messages when returning fmi2Pending status from any function.",
            "Log all messages.",};
    }    

}


