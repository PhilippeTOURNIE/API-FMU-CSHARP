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

using FMICsharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FMU2ManageCsharp
{
    //---------------------------------------------------------
    //REMARQUES IMPORTANTES
    // Je n'arrive pas à transmettre correctement les booléens
    // par référence
    //---------------------------------------------------------

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("f1158f17-a005-1605-4ae2-8c7039f2d445")]
    [ComVisible(true)]
    public class FMU2Manage
    {


        #region Fields

        /// <summary>
        /// Attention ne supporte le mot clé static en MEF !!
        /// </summary>
        [ImportMany]
        IEnumerable<Lazy<IFMI2, IFMI2_Metadata>> _ListTemplateFMUs;

        private static List<IFMI2> _ListFMUs = new List<IFMI2>();

        private static CompositionContainer _container;

        #endregion

        #region Tools Functions

        /// <summary>
        /// Chargement des plug in 
        /// Attention les assembly chargées ne sont pas déchargées
        /// TODO Mettre en place la classe AssemblyService pourcharger et décharger une assembly
        /// </summary>
        /// <param name="afmuResourceLocation"></param>
        private void LoadTemplateFMU(string afmuResourceLocation)
        {
            
            try
            {

                // An aggregate catalog that combines multiple catalogs
                var catalog = new AggregateCatalog();
                // Adds all the parts found in the same assembly as the Program class
                catalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));


                // Sécurité dans le cas d'un lien venant de c++
                afmuResourceLocation = afmuResourceLocation.Replace("file:///", "");

                if (!Directory.Exists(afmuResourceLocation))
                {
                    FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, string.Format("MEF : Ressource Location {0}", afmuResourceLocation));
                }

                //Ajoute le répertoire de plug in
                catalog.Catalogs.Add(new DirectoryCatalog(afmuResourceLocation));



                // Create the CompositionContainer with the parts in the catalog
                _container = new CompositionContainer(catalog);

                // Fill the imports of this object

                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "MEF CATALOG:" + compositionException.ToString());
            }

        }

        private static bool Tobool(int avalue)
        {
            return avalue == 0 ? false : true;
        }
        private static int ToInt(bool avalue)
        {
            return avalue ? 1 : 0;
        }

        #endregion




        /***************************************************
        Common Functions
        ****************************************************/

        #region DEBUGAGE

        /* Inquire version numbers of header files and setting logging status */
        public int SetDebugLogging(object c, int loggingOn, int nCategories, string[] categories)
        {
            return (int)((IFMI2)c).fmi2SetDebugLogging(Tobool(loggingOn), nCategories, categories);
        }

        #endregion


        /* Enter and exit initialization mode, terminate and reset */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanciateName"></param>
        /// <param name="fmuType"></param>
        /// <param name="fmuGUID"></param>
        /// <param name="fmuResourceLocation"></param>
        /// <param name="visible"></param>
        /// <param name="loggingOn"></param>
        /// <returns></returns>
        public object Instantiate(string instanciateName, int fmuType, string fmuGUID, string fmuResourceLocation, int visible, int loggingOn)
        {
            //FMICsharp.Log.Logger.ModeLog= loggingOn;

            _ListFMUs.Clear(); // Ne tolère qu'un FMU à la fois pour le moment

            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO,
                string.Format("Manage Instanciate :{0}  GUID {1}, resources : {2}"
                , instanciateName
                , fmuGUID,
                fmuResourceLocation));

            try
            {
                //Refresh the List of Avialiable FMU
                LoadTemplateFMU(fmuResourceLocation);
                if (_ListTemplateFMUs == null)
                {
                    FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "MEF :_ListTemplateFMUs , pointeur is null");
                    return null;
                }
                IFMI2 _ITypeFMU = (from extension in _ListTemplateFMUs where ((IFMI2_Metadata)extension.Metadata).GUID == fmuGUID select extension.Value).FirstOrDefault();
                if (_ITypeFMU == null)
                {
                    FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "MEF :FMU, pointeur is null");
                    return null;
                }
                IFMI2 _FMU = (IFMI2)Activator.CreateInstance(_ITypeFMU.GetType());
                _FMU.fmi2Instantiate(instanciateName, fmuType, fmuGUID, fmuResourceLocation, Tobool(visible), Tobool(loggingOn));
                _ListFMUs.Add(_FMU);

                if (_FMU == null)
                    FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "MEF :FMU, pointeur is null");

                return _FMU;

            }
            catch (Exception)
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.FATAL, string.Format("Instanciates FMU {0} failed", instanciateName));
                return null;
            }
        }

        public void FreeInstance(object c)
        {

            ((IFMI2)c).fmi2FreeInstance();


            // System.IO.File.WriteAllText("d:\\temp\\FMUTest.txt", fmuGUID);
            try
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "Start Remove");
                _ListFMUs.Remove((IFMI2)c);
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "End Remove");
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "nb IFMU" + _ListFMUs.Count.ToString());
            }
            catch (Exception e)
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "Failed Remove: " + e.Message);
            }


        }

        public int SetupExperiment(object c, int toleranceDefined, double tolerance, double startTime, int stopTimeDefined, double stopTime)
        {
            try
            {

                //_ListFMUs[0].fmi2SetupExperiment(Tobool(toleranceDefined), tolerance, startTime, Tobool(stopTimeDefined), stopTime);

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetupExperiment");
                return (int)((IFMI2)c).fmi2SetupExperiment(Tobool(toleranceDefined), tolerance, startTime, Tobool(stopTimeDefined), stopTime);
            }
            catch (Exception e)
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR,
                e.Message + " Stack Trace: " + e.StackTrace);
                return (int)fmi2Status.fmi2Error;
            }

        }
        public int EnterInitializationMode(object c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "EnterInitializationMode");
            return (int)((IFMI2)c).fmi2EnterInitializationMode();
        }
        public int ExitInitializationMode(object c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "ExitInitializationMode");
            return (int)((IFMI2)c).fmi2ExitInitializationMode();
        }
        public int Terminate(object c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "Terminate");
            return (int)((IFMI2)c).fmi2Terminate(); ;
        }
        public int Reset(object c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "Reset");
            return (int)((IFMI2)c).fmi2Reset();
        }

        /* Getting and setting variable values */
        public int GetReal(object c, int[] vr, int nvr, ref double[] value)
        {
            try
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetReal");
                return (int)((IFMI2)c).fmi2GetReal(vr, ref value);
            }
            catch (Exception e)
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "GetReal:" + e.Message);
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "_ListFMUs.Count()" + _ListFMUs.Count().ToString());
            }
            return 0;
        }
        public int GetInteger(object c, int[] vr, int nvr, ref int[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetInteger");
            return (int)((IFMI2)c).fmi2GetInteger(vr, ref value);
        }
        public int GetBoolean(object c, int[] vr, int nvr, ref int[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetBoolean");
            bool[] _value = new bool[value.Length];
            for (int i = 0; i < value.Length; i++)
                _value[i] = Tobool(value[i]);

            var res = ((IFMI2)c).fmi2GetBoolean(vr, ref  _value);
            for (int i = 0; i < value.Length; i++)
                value[i] = ToInt(_value[i]);
            return (int)res;

        }
        public int GetString(object c, int[] vr, int nvr, ref string[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, string.Format("\nGetString :value.count {0}\n", value.Length));
            var res = (int)((IFMI2)c).fmi2GetString(vr, ref value);
            //value[0] =string.Format( "CSTB {0}",nvr.ToString());
            return res;
        }
        public int SetReal(object c, int[] vr, int nvr, double[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetReal");
            return (int)((IFMI2)c).fmi2SetReal(vr, value);
        }
        public int SetInteger(object c, int[] vr, int nvr, int[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetInteger");
            return (int)((IFMI2)c).fmi2SetInteger(vr, value);
        }
        public int SetBoolean(object c, int[] vr, int nvr, int[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetBoolean");
            bool[] _value = new bool[value.Length];
            for (int i = 0; i < value.Length; i++)
                _value[i] = Tobool(value[i]);

            return (int)((IFMI2)c).fmi2SetBoolean(vr, _value);
        }
        public int SetString(object c, int[] vr, int nvr, string[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetString");
            return (int)((IFMI2)c).fmi2SetString(vr, value);
        }

        ///* Getting and setting the internal FMU state */
        public int GetFMUstate(IFMI2 c, ref  int aFMUstate)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetFMUstate");
            fmi2Status _fmi2Status = (fmi2Status)aFMUstate;
            var res = (int)((IFMI2)c).fmi2GetFMUstate(ref _fmi2Status);
            aFMUstate = (int)_fmi2Status;
            return res;
        }
        public int SetFMUstate(IFMI2 c, int aFMUstate)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetFMUstate");
            return (int)((IFMI2)c).fmi2SetFMUstate((fmi2Status)aFMUstate);
        }
        public int FreeFMUstate(IFMI2 c, int aFMUstate)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "FreeFMUstate");
            return (int)((IFMI2)c).fmi2FreeFMUstate((fmi2Status)aFMUstate);
        }
        public int SerializedFMUstateSize(IFMI2 c, int aFMUstate, ref int asize)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SerializedFMUstateSize");
            return (int)((IFMI2)c).fmi2SerializedFMUstateSize((fmi2Status)aFMUstate, ref asize);
        }
        public int SerializeFMUstate(IFMI2 c, int aFMUstate, ref byte[] serializeState, int size)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SerializeFMUstate");
            return (int)((IFMI2)c).fmi2SerializeFMUstate((fmi2Status)aFMUstate, ref serializeState);
        }
        public int DeSerializeFMUstate(IFMI2 c, ref byte[] serializeState, int size, int aFMUstate)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "DeSerializeFMUstate");
            return (int)((IFMI2)c).fmi2DeSerializeFMUstate(ref serializeState, (fmi2Status)aFMUstate);
        }

        ///* Getting partial derivatives */
        public int GetDirectionalDerivative(IFMI2 c, ref int[] vrUnknown_ref, int nUnknown,
            ref int[] vKnown, int nKnown, ref double[] dvKnown, ref double[] dvUnknown)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetDirectionalDerivative");
            return (int)((IFMI2)c).fmi2GetDirectionalDerivative(ref vrUnknown_ref, ref vKnown, ref dvKnown, ref dvUnknown);
        }

        ///***************************************************
        //Types for Functions for FMI2 for Model Exchange
        //****************************************************/

        /* Enter and exit the different modes */
        public int EnterEventMode(IFMI2 c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "EnterEventMode");
            return (int)((IFMI2)c).fmi2EnterEventMode();
        }
        public int NewDiscreteStates(IFMI2 c, ref  int newDiscreteStatesNeeded,
        ref int terminateSimulation,
        ref int nominalsOfContinuousStatesChanged,
        ref int valuesOfContinuousStatesChanged,
        ref int nextEventTimeDefined,
        ref double nextEventTime)
        {
            fmi2EventInfo _fmi2eventInfo = new fmi2EventInfo();
            _fmi2eventInfo.newDiscreteStatesNeeded = Tobool(newDiscreteStatesNeeded);
            _fmi2eventInfo.terminateSimulation = Tobool(terminateSimulation);
            _fmi2eventInfo.nominalsOfContinuousStatesChanged = Tobool(nominalsOfContinuousStatesChanged);
            _fmi2eventInfo.valuesOfContinuousStatesChanged = Tobool(valuesOfContinuousStatesChanged);
            _fmi2eventInfo.nextEventTimeDefined = Tobool(nextEventTimeDefined);
            _fmi2eventInfo.nextEventTime = nextEventTime;
            int res = (int)((IFMI2)c).fmi2NewDiscreteStates(ref _fmi2eventInfo);
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, String.Format("NewDiscreteStates {0}", _fmi2eventInfo.newDiscreteStatesNeeded));

            newDiscreteStatesNeeded = ToInt(_fmi2eventInfo.newDiscreteStatesNeeded);
            terminateSimulation = ToInt(_fmi2eventInfo.terminateSimulation);
            nominalsOfContinuousStatesChanged = ToInt(_fmi2eventInfo.nominalsOfContinuousStatesChanged);
            valuesOfContinuousStatesChanged = ToInt(_fmi2eventInfo.valuesOfContinuousStatesChanged);
            nextEventTimeDefined = ToInt(_fmi2eventInfo.nextEventTimeDefined);
            nextEventTime = _fmi2eventInfo.nextEventTime;
            return res;
        }
        public int EnterContinuousTimeMode(IFMI2 c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "EnterContinuousTimeMode");
            return (int)((IFMI2)c).fmi2EnterContinuousTimeMode();
        }
        public int CompletedIntegratorStep(IFMI2 c, int noSetFMUStatePriorToCurrentPoint, ref int enterEventMode, ref int terminateSimulation)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "CompletedIntegratorStep");
            bool _noSetFMUStatePriorToCurrentPoint = Tobool(noSetFMUStatePriorToCurrentPoint);
            bool _enterEventMode = Tobool(enterEventMode);
            bool _terminateSimulation = Tobool(terminateSimulation);
            var res = ((IFMI2)c).fmi2CompletedIntegratorStep(_noSetFMUStatePriorToCurrentPoint, ref _enterEventMode, ref _terminateSimulation);
            enterEventMode = ToInt(_enterEventMode);
            terminateSimulation = ToInt(_terminateSimulation);
            return (int)res;
        }

        /* Providing independent variables and re-initialization of caching */
        public int SetTime(IFMI2 c, double time)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetTime");
            return (int)((IFMI2)c).fmi2SetTime(time);
        }
        public int SetContinuousStates(IFMI2 c, double[] x, int nx)
        {
            for (int i = 0; i < nx; i++)
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, string.Format("\nSetContinuousStates {0}\n", x[i]));


            return (int)((IFMI2)c).fmi2SetContinuousStates(x);
        }

        /* Evaluation of the model equations */
        public int GetDerivatives(IFMI2 c, ref double[] derivatives, int nx)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetDerivatives");
            return (int)((IFMI2)c).fmi2GetDerivatives(ref derivatives);
        }
        public int GetEventIndicators(IFMI2 c, ref double[] eventIndicators, int ni)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetEventIndicators");
            return (int)((IFMI2)c).fmi2GetEventIndicators(ref eventIndicators);
        }
        public int GetContinuousStates(IFMI2 c, ref double[] x, int nx)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetContinuousStates");
            return (int)((IFMI2)c).fmi2GetContinuousStates(ref x);
        }
        public int GetNominalsOfContinuousStates(IFMI2 c, ref double[] x_nominal, int nx)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetNominalsOfContinuousStates");
            return (int)((IFMI2)c).fmi2GetNominalsOfContinuousStates(ref x_nominal);
        }


        //***************************************************
        //Functions for FMI2 for Co-Simulation
        //****************************************************/

        /* Simulating the slave */
        public int SetRealInputDerivatives(IFMI2 c, int[] vr, int[] order, double[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "SetRealInputDerivatives");
            return (int)((IFMI2)c).fmi2SetRealInputDerivatives(vr, order, value);
        }
        public int GetRealOutputDerivatives(IFMI2 c, int[] vr, int[] order, ref double[] value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetRealOutputDerivatives");
            return (int)((IFMI2)c).fmi2GetRealOutputDerivatives(vr, order, ref value);
        }

        public int DoStep(IFMI2 c, double currentCommunicationPoint, double communicationStepSize, int noSetFMUStatePriorToCurrentPoint)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "DoStep");
            return (int)((IFMI2)c).fmi2DoStep(currentCommunicationPoint, communicationStepSize, Tobool(noSetFMUStatePriorToCurrentPoint));
        }

        public int CancelStep(IFMI2 c)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "CancelStep");
            return (int)((IFMI2)c).fmi2CancelStep();
        }

        /* Inquire slave status */
        public int GetStatus(IFMI2 c, int s, ref int value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetStatus");
            return (int)((IFMI2)c).fmi2GetStatus((fmi2StatusKind)s, ref value);
        }
        public int GetRealStatus(IFMI2 c, int s, ref double value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetRealStatus");
            return (int)((IFMI2)c).fmi2GetRealStatus((fmi2StatusKind)s, ref value);
        }
        public int GetIntegerStatus(IFMI2 c, int s,
            ref int value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetIntegerStatus");
            return (int)((IFMI2)c).fmi2GetIntegerStatus((fmi2StatusKind)s, ref value);
        }
        public int GetBooleanStatus(IFMI2 c, int s,
            ref int value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetBooleanStatus");
            bool _value = Tobool(value);
            var res = ((IFMI2)c).fmi2GetBooleanStatus((fmi2StatusKind)s, ref _value);
            value = ToInt(_value);
            return (int)res;
        }
        public int GetStringStatus(IFMI2 c, int s,
            ref string value)
        {
            FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO, "GetStringStatus");
            return (int)((IFMI2)c).fmi2GetStringStatus((fmi2StatusKind)s, ref value);
        }


    }
}