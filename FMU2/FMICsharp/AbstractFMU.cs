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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FMICsharp
{
    public abstract class AbstractFMU : IFMI2
    {

        #region Property

        /// <summary>
        /// Define Categorie specific Qtronic
        /// </summary>
        protected string[] _logCategoriesNamesQtronic = new string[] { "logAll", "logError", "logFmiCall", "logEvent" };
        
        /// <summary>
        /// Offical FMI2.0
        /// </summary>
        protected string[] _logCategoriesNames = new string[Enum.GetNames(typeof(FMICsharp.Log.CategorieNameEnum)).Length];
        

        protected bool _loggingOn;

        protected bool[] _CategorieList = new bool[Enum.GetNames(typeof(fmi2CategorieQtronic)).Length];

        /// <summary>
        /// Real Array
        /// </summary>
        protected abstract List<double> R { get; }
        /// <summary>
        /// Integer Array
        /// </summary>
        protected abstract List<int> I { get; }
        /// <summary>
        /// Bool Array
        /// </summary>
        protected abstract List<bool> B { get; }
        /// <summary>
        /// String Array
        /// </summary>
        protected abstract List<string> S { get; }

        /// <summary>
        /// Data Model FMU from Xsd 
        /// </summary>
        protected fmiModelDescription _DataModel;

        /// <summary>
        /// Name of the FMU
        /// </summary>
        protected string _InstanceName { get; set; }
        /// <summary>
        /// Type FMU
        /// </summary>
        protected fmi2Type _FMUType { get; set; }
        /// <summary>
        /// GUID of the FMU
        /// </summary>
        protected string _FMUGUID { get; set; }


        protected bool _isPositive;
        /// <summary>
        /// Time simulation
        /// </summary>
        protected double _time;
        /// <summary>
        /// Component environnement
        /// </summary>
        protected string _componentEnvironment;

        /// <summary>
        /// State of model during simulation
        /// </summary>
        protected fmi2ModelState _state;

        /// <summary>
        /// Event information
        /// </summary>
        protected fmi2EventInfo _eventInfo = new fmi2EventInfo();

        /// <summary>
        /// is Dirty Values
        /// </summary>
        protected bool _isDirtyValues; // !0 is true

        /// <summary>
        /// Number of the State
        /// </summary>
        protected int[] _vrStates{get; set;}  //  [NUMBER_OF_STATES]
        
        /// <summary>
        /// Correspond exactement au fichier xml description
        /// </summary>
        private int NUMBER_OF_REALS
        {
            get { return R.Count; }
        }
        private int NUMBER_OF_INTEGERS
        {
            get { return I.Count; }
        }
        private int NUMBER_OF_BOOLEANS
        {
            get { return B.Count; }
        }
        private int NUMBER_OF_STRINGS { get { return S.Count; } }

        protected abstract int NUMBER_OF_STATES { get; }

        protected abstract int NUMBER_OF_EVENT_INDICATORS { get; }


        #endregion


        #region Fonctions FMU
        public virtual fmi2Status fmi2SetDebugLogging(bool aloggingOn, int anCategories, string[] acategories)
        {
            //TODO Initialise les catégories offciels elles devront également être placé dans la description XML
            
            
                
            
            

            _loggingOn = aloggingOn;

            // reset all categories 
            for (int i = 0; i < Enum.GetNames(typeof(fmi2CategorieQtronic)).Length; i++)
                _CategorieList[i] = false;

            // si rien est spécifié alors on met tout à logging on !
            if (anCategories == 0)
            {
                for (int i = 0; i < Enum.GetNames(typeof(fmi2CategorieQtronic)).Length; i++)
                    _CategorieList[i] = _loggingOn;
            }
            else
            {
                // set specific categories on
                for (int i = 0; i < anCategories; i++)
                {
                    bool categoryFound = false;
                    for (int j = 0; j < Enum.GetNames(typeof(fmi2CategorieQtronic)).Length; j++)
                    {
                        if (string.Compare(_logCategoriesNamesQtronic[j], acategories[i]) == 0)
                        {
                            _CategorieList[j] = _loggingOn;
                            categoryFound = true;
                            break;
                        }
                    }
                    if (!categoryFound)
                    {
                        //TODO Call Functions
                        //functions->logger(comp->componentEnvironment, comp->instanceName, fmi2Status.fmi2Warning,
                        //    Model.LogCategories.Category[(int)fmi2Categorie.LOG_ERROR].name,
                        //    "logging category '%s' is not supported by model", acategories[i]);
                    }
                }
            }

            return fmi2Status.fmi2OK;
        }

        public virtual IFMI2 fmi2Instantiate(string instanciateName, int fmuType, string fmuGUID, string fmuResourceLocation, bool visible, bool loggingOn)
        {
            try
            {
                _time = 0; // overwrite in fmi2SetupExperiment, fmi2SetTime
                _InstanceName = instanciateName;
                _FMUType = (fmi2Type)fmuType;
                _FMUGUID = fmuGUID;
                //TODO à résoudre comp->functions = functions;
                //TODO _componentEnvironment = functions->componentEnvironment;
                _loggingOn = loggingOn;
                _state = fmi2ModelState.modelInstantiated;
                setStartValues(); // to be implemented by the includer of this file
                _isDirtyValues = true; // because we just called setStartValues

                _eventInfo.newDiscreteStatesNeeded = false;
                _eventInfo.terminateSimulation = false;
                _eventInfo.nominalsOfContinuousStatesChanged = false;
                _eventInfo.valuesOfContinuousStatesChanged = false;
                _eventInfo.nextEventTimeDefined = false;
                _eventInfo.nextEventTime = 0;

                

            }
            catch (Exception ee)
            {
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2Instantiate: " + ee.Message);
                return null;
            }
            return this;
        }

        public virtual void fmi2FreeInstance()
        {
            FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2FreeInstance");
        }

        public virtual fmi2Status fmi2SetupExperiment(bool toleranceDefined, double tolerance, double startTime, bool stopTimeDefined, double stopTime)
        {
            try
            {
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO,
                  String.Format("fmi2SetupExperiment: toleranceDefined {0},tolerance {1},startTime {2},stopTimeDefined {3}, stopTime: {4}",
                  toleranceDefined.ToString(),
                  tolerance.ToString(),
                  startTime.ToString(),
                  stopTimeDefined.ToString(),
                  stopTime.ToString()));

                // ignore arguments: stopTimeDefined, stopTime

                if (invalidState("fmi2SetupExperiment", (int)Mask.MASK_fmi2SetupExperiment))
                {                    
                    return fmi2Status.fmi2Error;
                }
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetupExperiment: toleranceDefined={0} tolerance={1}", toleranceDefined.ToString(), tolerance.ToString()));

                _time = startTime;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "invalidState: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }

        }

        public virtual fmi2Status fmi2EnterInitializationMode()
        {
            try
            {
                if (invalidState("fmi2EnterInitializationMode", (int)Mask.MASK_fmi2EnterInitializationMode))
                {
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR,
                        string.Format("fmi2EnterInitializationMode: _state {0},Mask.MASK_fmi2EnterInitializationMode {1}", _state.ToString(), Mask.MASK_fmi2EnterInitializationMode.ToString()));
                    return fmi2Status.fmi2Error;
                }

                _state = fmi2ModelState.modelInitializationMode;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2EnterInitializationMode: _state {0}", _state.ToString()));

                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2EnterInitializationMode: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2ExitInitializationMode()
        {
            try
            {
                if (invalidState("fmi2ExitInitializationMode", (int)Mask.MASK_fmi2ExitInitializationMode))
                {
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR,
                      string.Format("fmi2ExitInitializationMode: _state {0}, Mask.MASK_fmi2ExitInitializationMode {1}", _state.ToString(), Mask.MASK_fmi2ExitInitializationMode.ToString()));
                    return fmi2Status.fmi2Error;
                }

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2ExitInitializationMode: _isDirtyValues {0}", _isDirtyValues.ToString()));

                // if values were set and no fmi2GetXXX triggered update before,
                // ensure calculated values are updated now
                if (_isDirtyValues)
                {
                    calculateValues();
                    _isDirtyValues = false;
                }

                if (_FMUType == fmi2Type.fmi2ModelExchange)
                    _state = fmi2ModelState.modelEventMode;
                else
                    _state = fmi2ModelState.modelStepComplete;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2EnterInitializationMode: _state {0}", _state.ToString()));

                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2ExitInitializationMode: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2Terminate()
        {
            try
            {
                if (invalidState("fmi2Terminate", (int)Mask.MASK_fmi2Terminate))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2Terminate");

                _state = fmi2ModelState.modelTerminated;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "invalidState: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2Reset()
        {
            try
            {
                if (invalidState("fmi2Reset", (int)Mask.MASK_fmi2Reset))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2Reset");

                _state = fmi2ModelState.modelInstantiated;
                setStartValues(); // to be implemented by the includer of this file
                _isDirtyValues = true; // because we just called setStartValues

                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2Reset: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetReal(int[] vr, ref double[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetReal", (int)Mask.MASK_fmi2GetReal))
                    return fmi2Status.fmi2Error;

                if (nullPointer("fmi2GetReal:", "vr[]", vr))
                    return fmi2Status.fmi2Error;

                if (nullPointer("fmi2GetReal:", "value[]", value))
                    return fmi2Status.fmi2Error;

                if (vr.Length > 0 && _isDirtyValues)
                {
                    calculateValues();
                    _isDirtyValues = false;
                }
                if (NUMBER_OF_REALS > 0)
                {
                    for (i = 0; i < vr.Length; i++)
                    {
                        if (vrOutOfRange("fmi2GetReal", vr[i], NUMBER_OF_REALS))
                            return fmi2Status.fmi2Error;

                        value[i] = getReal(vr[i]); // to be implemented by the includer of this file
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetReal: index {0}, value {1}", vr[i].ToString(), value[i].ToString()));

                    }
                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetReal: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetInteger(int[] vr, ref int[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetInteger", (int)Mask.MASK_fmi2GetInteger))
                {
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("fmi2GetInteger: {0}", _state.ToString()));
                    return fmi2Status.fmi2Error;
                }
                if (nullPointer("fmi2GetInteger:", "vr[]", vr))
                    return fmi2Status.fmi2Error;

                if (nullPointer("fmi2GetInteger:", "value[]", value))
                    return fmi2Status.fmi2Error;

                if (vr.Length > 0 && _isDirtyValues)
                {
                    calculateValues();
                    _isDirtyValues = false;
                }
                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2GetInteger", vr[i], NUMBER_OF_INTEGERS))
                        return fmi2Status.fmi2Error;

                    value[i] = I[vr[i]];
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetInteger: index {0}, value {1}", vr[i].ToString(), value[i].ToString()));
                }

                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetInteger: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetBoolean(int[] vr, ref bool[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetBoolean", (int)Mask.MASK_fmi2GetBoolean))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetBoolean:", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetBoolean:", "value[]", value))
                    return fmi2Status.fmi2Error;

                if (vr.Length > 0 && _isDirtyValues)
                {
                    calculateValues();
                    _isDirtyValues = false;
                }
                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2GetBoolean", vr[i], NUMBER_OF_BOOLEANS))
                        return fmi2Status.fmi2Error;
                    value[i] = B[vr[i]];
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetBoolean: index {0}, value {1}", vr[i].ToString(), value[i].ToString()));

                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetBoolean: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetString(int[] vr, ref string[] value)
        {
            try
            {
                int i;

                if (invalidState("fmi2GetString", (int)Mask.MASK_fmi2GetString))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetString:", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetString:", "value[]", value))
                    return fmi2Status.fmi2Error;

                if (vr.Length > 0 && _isDirtyValues)
                {
                    calculateValues();
                    _isDirtyValues = false;
                }
                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2GetString", vr[i], NUMBER_OF_STRINGS))
                        return fmi2Status.fmi2Error;
                    value[i] = S[vr[i]];

                    //FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetString: S0 {0}, S1 {1}", S[0].ToString(), S[1].ToString()));


                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetString: index {0}, value {1}", vr[i].ToString(), value[i].ToString()));

                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetString: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetReal(int[] vr, double[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2SetReal", (int)Mask.MASK_fmi2SetReal))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetReal", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetReal", "value[]", value))
                    return fmi2Status.fmi2Error;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetReal: nvr = {0}", vr.Length.ToString()));

                // no check whether setting the value is allowed in the current state
                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2SetReal", vr[i], NUMBER_OF_REALS))
                        return fmi2Status.fmi2Error;
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetReal: r[{0}] = {1}", vr[i], value[i]));
                    R[vr[i]] = value[i];
                }
                if (vr.Length > 0) _isDirtyValues = true;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetReal: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetInteger(int[] vr, int[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2SetInteger", (int)Mask.MASK_fmi2SetInteger))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetInteger", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetInteger", "value[]", value))
                    return fmi2Status.fmi2Error;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetInteger: nvr = {0}", vr.Length.ToString()));

                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2SetInteger", vr[i], NUMBER_OF_INTEGERS))
                        return fmi2Status.fmi2Error;

                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetInteger: r[{0}] = {1}", vr[i], value[i]));

                    I[vr[i]] = value[i];
                }
                if (vr.Length > 0) _isDirtyValues = true;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetInteger: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetBoolean(int[] vr, bool[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2SetBoolean", (int)Mask.MASK_fmi2SetBoolean))
                    return fmi2Status.fmi2Error;

                if (nullPointer("fmi2SetBoolean", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetBoolean", "value[]", value))
                    return fmi2Status.fmi2Error;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetBoolean: nvr = {0}", vr.Length.ToString()));

                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2SetBoolean", vr[i], NUMBER_OF_BOOLEANS))
                        return fmi2Status.fmi2Error;
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetBoolean: r[{0}] = {1}", vr[i], value[i]));
                    B[vr[i]] = value[i];
                }
                if (vr.Length > 0) _isDirtyValues = true;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetBoolean: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetString(int[] vr, string[] value)
        {
            try
            {
                int i;

                if (invalidState("fmi2SetString", (int)Mask.MASK_fmi2SetString))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetString", "vr[]", vr))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetString", "value[]", value))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetString: nvr = {0}", vr.Length.ToString()));
                for (i = 0; i < vr.Length; i++)
                {
                    if (vrOutOfRange("fmi2SetString", vr[i], NUMBER_OF_STRINGS))
                        return fmi2Status.fmi2Error;
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetString: r[{0}] = {1}", vr[i], value[i]));
                    S[vr[i]] = value[i];
                }

                if (vr.Length > 0)
                    _isDirtyValues = true;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetString: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetFMUstate(ref fmi2Status FMUstate)
        {
            try
            {
                return unsupportedFunction("fmi2GetFMUstate", (int)Mask.MASK_fmi2GetFMUstate);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetFMUstate: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetFMUstate(fmi2Status FMUstate)
        {
            try
            {
                return unsupportedFunction("fmi2SetFMUstate", (int)Mask.MASK_fmi2SetFMUstate);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetFMUstate: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2FreeFMUstate(fmi2Status FMUstate)
        {
            try
            {
                return unsupportedFunction("fmi2FreeFMUstate", (int)Mask.MASK_fmi2FreeFMUstate);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2FreeFMUstate: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SerializedFMUstateSize(fmi2Status FMUstate, ref int size)
        {
            try
            {
                return unsupportedFunction("fmi2SerializedFMUstateSize", (int)Mask.MASK_fmi2SerializedFMUstateSize);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SerializedFMUstateSize: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SerializeFMUstate(fmi2Status FMUstate, ref byte[] serializeState)
        {
            try
            {
                return unsupportedFunction("fmi2SerializeFMUstate", (int)Mask.MASK_fmi2SerializeFMUstate);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SerializeFMUstate: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2DeSerializeFMUstate(ref byte[] serializeState, fmi2Status FMUstate)
        {
            try
            {
                return unsupportedFunction("fmi2DeSerializeFMUstate", (int)Mask.MASK_fmi2DeSerializeFMUstate);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2DeSerializeFMUstate: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetDirectionalDerivative(ref int[] vrUnknown_ref, ref int[] vKnown, ref double[] dvKnown, ref double[] dvUnknown)
        {
            try
            {
                return unsupportedFunction("fmi2GetDirectionalDerivative", (int)Mask.MASK_fmi2GetDirectionalDerivative);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetDirectionalDerivative: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        // ---------------------------------------------------------------------------
        // Functions for FMI2 for Model Exchange
        // ---------------------------------------------------------------------------
        public virtual fmi2Status fmi2EnterEventMode()
        {
            try
            {
                if (invalidState("fmi2EnterEventMode", (int)Mask.MASK_fmi2EnterEventMode))
                    return fmi2Status.fmi2Error;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2EnterEventMode");
                _state = fmi2ModelState.modelEventMode;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2EnterEventMode: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2NewDiscreteStates(ref fmi2EventInfo fmi2eventInfo)
        {
            try
            {
                bool timeEvent = false;
                if (invalidState("fmi2NewDiscreteStates", (int)Mask.MASK_fmi2NewDiscreteStates))
                {
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("fmi2NewDiscreteStates: state {0}, mask {1}", _state.ToString(), Mask.MASK_fmi2NewDiscreteStates.ToString()));
                    return fmi2Status.fmi2Error;
                }


                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2NewDiscreteStates: Start"));

                _eventInfo.newDiscreteStatesNeeded = false;
                _eventInfo.terminateSimulation = false;
                _eventInfo.nominalsOfContinuousStatesChanged = false;
                _eventInfo.valuesOfContinuousStatesChanged = false;

                if (_eventInfo.nextEventTimeDefined && _eventInfo.nextEventTime <= _time)
                {
                    timeEvent = true;
                }

                eventUpdate(timeEvent);

                // copy internal eventInfo of component to output eventInfo
                fmi2eventInfo.newDiscreteStatesNeeded = _eventInfo.newDiscreteStatesNeeded;
                fmi2eventInfo.terminateSimulation = _eventInfo.terminateSimulation;
                fmi2eventInfo.nominalsOfContinuousStatesChanged = _eventInfo.nominalsOfContinuousStatesChanged;
                fmi2eventInfo.valuesOfContinuousStatesChanged = _eventInfo.valuesOfContinuousStatesChanged;
                fmi2eventInfo.nextEventTimeDefined = _eventInfo.nextEventTimeDefined;
                fmi2eventInfo.nextEventTime = _eventInfo.nextEventTime;



                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2NewDiscreteStates: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2EnterContinuousTimeMode()
        {
            try
            {
                if (invalidState("fmi2EnterContinuousTimeMode", (int)Mask.MASK_fmi2EnterContinuousTimeMode))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2EnterContinuousTimeMode");

                _state = fmi2ModelState.modelContinuousTimeMode;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2EnterContinuousTimeMode: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2CompletedIntegratorStep(bool noSetFMUStatePriorToCurrentPoint, ref bool enterEventMode, ref bool terminateSimulation)
        {
            try
            {
                if (invalidState("fmi2CompletedIntegratorStep", (int)Mask.MASK_fmi2CompletedIntegratorStep))
                    return fmi2Status.fmi2Error;

                enterEventMode = false;
                terminateSimulation = false;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2CompletedIntegratorStep: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetTime(double time)
        {
            try
            {
                if (invalidState("fmi2SetTime", (int)Mask.MASK_fmi2SetTime))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetTime: time={0}", time.ToString()));
                _time = time;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetTime: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2SetContinuousStates(double[] x)
        {
            try
            {
                int i;
                if (invalidState("fmi2SetContinuousStates", (int)Mask.MASK_fmi2SetContinuousStates))
                    return fmi2Status.fmi2Error;
                if (invalidNumber("fmi2SetContinuousStates", "nx", x.Length, NUMBER_OF_STATES))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2SetContinuousStates", "x[]", x))
                {
                    return fmi2Status.fmi2Error;
                }
                if (NUMBER_OF_REALS > 0)
                {
                    for (i = 0; i < x.Length; i++)
                    {
                        int vr = _vrStates[i];
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetContinuousStates: r[{0}]={1}", vr.ToString(), x[i].ToString()));
                        R[vr] = x[i];
                    }
                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetContinuousStates: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetDerivatives(ref double[] derivatives)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetDerivatives", (int)Mask.MASK_fmi2GetDerivatives))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetDerivatives", "derivatives[]", derivatives))
                    return fmi2Status.fmi2Error;
                if (invalidNumber("fmi2GetDerivatives", "nx", derivatives.Length, NUMBER_OF_STATES))
                    return fmi2Status.fmi2Error;

                if (NUMBER_OF_STATES > 0)
                {
                    for (i = 0; i < derivatives.Length; i++)
                    {
                        int vr = _vrStates[i] + 1;
                        derivatives[i] = getReal(vr); // to be implemented by the includer of this file
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetDerivatives: r[{0}]={1}", vr.ToString(), derivatives[i].ToString()));
                    }
                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetDerivatives: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetEventIndicators(ref double[] eventIndicators)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetEventIndicators", (int)Mask.MASK_fmi2GetEventIndicators))
                    return fmi2Status.fmi2Error;
                if (invalidNumber("fmi2GetEventIndicators", "ni", eventIndicators.Length, NUMBER_OF_EVENT_INDICATORS))
                    return fmi2Status.fmi2Error;

                if (NUMBER_OF_EVENT_INDICATORS > 0)
                {
                    for (i = 0; i < eventIndicators.Length; i++)
                    {
                        eventIndicators[i] = getEventIndicator(i); // to be implemented by the includer of this file
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetEventIndicators: z[{0}]={1}", i.ToString(), eventIndicators[i].ToString()));

                    }
                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetEventIndicators: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetContinuousStates(ref double[] states)
        {
            try
            {
                int i;

                if (invalidState("fmi2GetContinuousStates", (int)Mask.MASK_fmi2GetContinuousStates))
                    return fmi2Status.fmi2Error;

                if (invalidNumber("fmi2GetContinuousStates", "nx", states.Length, NUMBER_OF_STATES))
                    return fmi2Status.fmi2Error;

                if (nullPointer("fmi2GetContinuousStates", "states[]", states))
                    return fmi2Status.fmi2Error;
                if (NUMBER_OF_REALS > 0)
                {
                    for (i = 0; i < states.Length; i++)
                    {
                        int vr = _vrStates[i];
                        states[i] = getReal(vr); // to be implemented by the includer of this file
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetContinuousStates: r[{0}]={1}", vr.ToString(), states[i].ToString()));

                    }
                }
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetContinuousStates: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetNominalsOfContinuousStates(ref double[] x_nominal)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetNominalsOfContinuousStates", (int)Mask.MASK_fmi2GetNominalsOfContinuousStates))
                    return fmi2Status.fmi2Error;
                if (nullPointer("fmi2GetNominalContinuousStates", "x_nominal[]", x_nominal))
                    return fmi2Status.fmi2Error;
                if (invalidNumber("fmi2GetNominalContinuousStates", "nx", x_nominal.Length, NUMBER_OF_STATES))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetNominalContinuousStates: x_nominal[0..{0}]= 1.0", (x_nominal.Length - 1).ToString()));

                for (i = 0; i < x_nominal.Length; i++)
                    x_nominal[i] = 1;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetNominalsOfContinuousStates: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        // ---------------------------------------------------------------------------
        // Functions for FMI for Co-Simulation
        // ---------------------------------------------------------------------------
        public virtual fmi2Status fmi2SetRealInputDerivatives(int[] vr, int[] order, double[] value)
        {
            try
            {
                if (invalidState("fmi2SetRealInputDerivatives", (int)Mask.MASK_fmi2SetRealInputDerivatives))
                {
                    return fmi2Status.fmi2Error;
                }
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2SetRealInputDerivatives: nvr= {0}", vr.Length.ToString()));
                //FILTERED_LOG(comp, fmi2Error, LOG_ERROR, "fmi2SetRealInputDerivatives: ignoring function call."
                //" This model cannot interpolate inputs: canInterpolateInputs=\"fmi2False\"")
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2SetRealInputDerivatives" + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetRealOutputDerivatives(int[] vr, int[] order, ref double[] value)
        {
            try
            {
                int i;
                if (invalidState("fmi2GetRealOutputDerivatives", (int)Mask.MASK_fmi2GetRealOutputDerivatives))
                    return fmi2Status.fmi2Error;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2GetRealOutputDerivatives: nvr= {0}", vr.Length.ToString()));
                //FILTERED_LOG(comp, fmi2OK, LOG_FMI_CALL, "fmi2GetRealOutputDerivatives: nvr= %d", nvr)
                //FILTERED_LOG(comp, fmi2Error, LOG_ERROR,"fmi2GetRealOutputDerivatives: ignoring function call."
                //" This model cannot compute derivatives of outputs: MaxOutputDerivativeOrder=\"0\"")
                for (i = 0; i < vr.Length; i++)
                    value[i] = 0;
                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetRealOutputDerivatives: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2DoStep(double currentCommunicationPoint, double communicationStepSize, bool noSetFMUStatePriorToCurrentPoint)
        {
            try
            {
                double h = communicationStepSize / 10;
                int k, i;
                const int n = 10; // how many Euler steps to perform for one do step
                double[] prevState = new double[Math.Max(NUMBER_OF_STATES, 1)];
                double[] prevEventIndicators = new double[Math.Max(NUMBER_OF_EVENT_INDICATORS, 1)];
                int stateEvent = 0;
                bool timeEvent = false;

                if (invalidState("fmi2DoStep", (int)Mask.MASK_fmi2DoStep))
                    return fmi2Status.fmi2Error;

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2DoStep: currentCommunicationPoint = {0}, communicationStepSize = {1},noSetFMUStatePriorToCurrentPoint = fmi2{2}",
                currentCommunicationPoint.ToString(), communicationStepSize.ToString(), noSetFMUStatePriorToCurrentPoint.ToString()));

                if (communicationStepSize <= 0)
                {
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("fmi2DoStep: communication step size must be > 0. Fount {0}.", communicationStepSize.ToString()));
                    _state = fmi2ModelState.modelError;
                    return fmi2Status.fmi2Error;
                }

                if (NUMBER_OF_EVENT_INDICATORS > 0)
                {
                    // initialize previous event indicators with current values
                    for (i = 0; i < NUMBER_OF_EVENT_INDICATORS; i++)
                    {
                        prevEventIndicators[i] = getEventIndicator(i);
                    }
                }

                // break the step into n steps and do forward Euler.
                _time = currentCommunicationPoint;
                for (k = 0; k < n; k++)
                {
                    _time += h;

                    if (NUMBER_OF_REALS > 0)
                    {
                        for (i = 0; i < NUMBER_OF_STATES; i++)
                        {
                            prevState[i] = R[_vrStates[i]];
                        }
                        for (i = 0; i < NUMBER_OF_STATES; i++)
                        {
                            int vr = _vrStates[i];
                            R[vr] += h * getReal(vr + 1); // forward Euler step
                        }
                    }

                    if (NUMBER_OF_EVENT_INDICATORS > 0)
                    {
                        // check for state event
                        for (i = 0; i < NUMBER_OF_EVENT_INDICATORS; i++)
                        {
                            double ei = getEventIndicator(i);
                            if (ei * prevEventIndicators[i] < 0)
                            {
                                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2DoStep: state event at {0}, z{1} crosses zero -{2}-", _time.ToString(), i.ToString(), (ei < 0) ? "\\" : "/"));
                                stateEvent++;
                            }
                            prevEventIndicators[i] = ei;
                        }
                    }
                    // check for time event
                    if (_eventInfo.nextEventTimeDefined && ((_time - _eventInfo.nextEventTime) > -0.0000000001))
                    {
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("fmi2DoStep: time event detected at {0}", _time.ToString()));
                        timeEvent = true;
                    }

                    if (int2bool(stateEvent) || timeEvent)
                    {
                        eventUpdate(timeEvent);
                        timeEvent = false;
                        stateEvent = 0;
                    }

                    // terminate simulation, if requested by the model in the previous step
                    if (_eventInfo.terminateSimulation)
                    {
                        FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("fmi2DoStep: model requested termination at t={0}", _time.ToString()));
                        _state = fmi2ModelState.modelStepFailed;
                        return fmi2Status.fmi2Discard; // enforce termination of the simulation loop
                    }
                }

                return fmi2Status.fmi2OK;
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2DoStep: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2CancelStep()
        {
            try
            {
                if (invalidState("fmi2CancelStep", (int)Mask.MASK_fmi2CancelStep))
                {
                    // always fmi2CancelStep is invalid, because model is never in modelStepInProgress state.
                    return fmi2Status.fmi2Error;
                }
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, "fmi2CancelStep");

                return fmi2Status.fmi2OK;
            }

            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2CancelStep: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetStatus(fmi2StatusKind s, ref int value)
        {
            try
            {
                return getStatus("fmi2GetStatus", s);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetStatus: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetRealStatus(fmi2StatusKind s, ref double value)
        {
            try
            {
                if (s == fmi2StatusKind.fmi2LastSuccessfulTime)
                {
                    if (invalidState("fmi2GetRealStatus", (int)Mask.MASK_fmi2GetRealStatus))
                        return fmi2Status.fmi2Error;
                    value = _time;
                    return fmi2Status.fmi2OK;
                }
                return getStatus("fmi2GetRealStatus", s);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetRealStatus: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetIntegerStatus(fmi2StatusKind s, ref  int value)
        {
            try
            {
                return getStatus("fmi2GetIntegerStatus", s);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetBooleanStatus(fmi2StatusKind s, ref bool value)
        {
            try
            {
                if (s == fmi2StatusKind.fmi2Terminated)
                {
                    if (invalidState("fmi2GetBooleanStatus", (int)Mask.MASK_fmi2GetBooleanStatus))
                        return fmi2Status.fmi2Error;
                    value = _eventInfo.terminateSimulation;
                    return fmi2Status.fmi2OK;
                }
                return getStatus("fmi2GetBooleanStatus", s);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetBooleanStatus: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        public virtual fmi2Status fmi2GetStringStatus(fmi2StatusKind s, ref string value)
        {
            try
            {
                return getStatus("fmi2GetStringStatus", s);
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, "fmi2GetStringStatus: " + ee.Message);
                return fmi2Status.fmi2Fatal;
            }
        }

        #endregion

        #region Méthode FMU

        #region Fonctions
        /// <summary>
        /// Appelé par fmi2GetReal, fmi2GetContinuousStates and fmi2GetDerivatives
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract double getReal(int index);
        /// <summary>
        /// Calcul des valeurs
        /// </summary>
        protected abstract void calculateValues();
        /// <summary>
        /// Initialise les valeurs
        /// </summary>
        protected abstract void setStartValues();
        /// <summary>
        /// 
        /// </summary>        
        /// <param name="isTimeEvent"></param>
        protected abstract void eventUpdate(bool isTimeEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        protected abstract double getEventIndicator(int z);
        #endregion

        #endregion

        #region Méthode interne




        private bool invalidState(string f, int statesExpected)
        {            
            int res = (int)_state & statesExpected;
            
            if (!(int2bool(res)))
            {            
                _state = fmi2ModelState.modelError;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Illegal call sequence.", f));
                return true;
            }
            return false;
        }

        private fmi2Status unsupportedFunction(string fName, int statesExpected)
        {
            if (invalidState(fName, statesExpected))
                return fmi2Status.fmi2Error;
            FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Function not implemented.", fName));
            return fmi2Status.fmi2Error;
        }

        private fmi2Status getStatus(string name, fmi2StatusKind s)
        {
            string[] statusKind = new string[3] { "fmi2DoStepStatus", "fmi2PendingStatus", "fmi2LastSuccessfulTime" };

            if (invalidState("fmi2GetStatus", (int)Mask.MASK_fmi2GetStatus)) // all get status have the same MASK_fmi2GetStatus
                return fmi2Status.fmi2Error;
            FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.INFO, string.Format("{0}: fmi2StatusKind = {1}", name, s.ToString()));

            switch (s)
            {
                case fmi2StatusKind.fmi2DoStepStatus:
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}:Can be called with fmi2DoStepStatus when fmi2DoStep returned fmi2Pending. This is not the case.", name));
                    break;
                case fmi2StatusKind.fmi2PendingStatus:
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Can be called with fmi2PendingStatus when fmi2DoStep returned fmi2Pending. This is not the case.", name));
                    break;
                case fmi2StatusKind.fmi2LastSuccessfulTime:
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Can be called with fmi2LastSuccessfulTime when fmi2DoStep returned fmi2Discard. This is not the case.", name));
                    break;
                case fmi2StatusKind.fmi2Terminated:
                    FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Can be called with fmi2Terminated when fmi2DoStep returned fmi2Discard. This is not the case.", name));
                    break;
            }
            return fmi2Status.fmi2Discard;
        }

        #endregion

        #region Tools
        protected bool int2bool(int value)
        {
            return value == 0 ? false : true;
        }
        protected bool nullPointer(string f, string arg, object p)
        {
            if (p == null)
            {
                _state = fmi2ModelState.modelError;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: null pointer .{1}", f, arg));
                return true;
            }
            return false;
        }

        protected bool vrOutOfRange(string f, int vr, int end)
        {
            if (vr >= end)
            {
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR, string.Format("{0}: Illegal value reference {1}.", f, vr.ToString()));
                _state = fmi2ModelState.modelError;
                return true;
            }
            return false;
        }
        protected bool invalidNumber(string f, string arg, int n, int nExpected)
        {
            if (n != nExpected)
            {
                _state = fmi2ModelState.modelError;
                FMICsharp.Log.Logger.WriteLog(Log.LogLevelL4N.ERROR,
                string.Format(" {0}: Invalid argument {1} = {2}. Expected {3}.", f, arg, n.ToString(), nExpected.ToString()));
                return true;
            }
            return false;
        }
        #endregion

    }
}
