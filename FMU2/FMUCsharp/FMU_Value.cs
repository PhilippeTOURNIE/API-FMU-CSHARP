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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMUCsharp
{


    [Export(typeof(IFMI2))]    
    [ExportMetadata("GUID","{8c4e810f-3df3-4a00-8276-176fa3c9f004}")]
    [ExportMetadata("Name", "Value")]
    [ExportMetadata("Version", "2.0  Apr.9, 2014")]    
    public class FMU_Value : AbstractFMU 
    {
        /*XDocument projectElement = XDocument.Load(fmuResourceLocation);
           XmlReader reader = projectElement.CreateReader();
           XmlSerializer mySerializer = new XmlSerializer(typeof(fmiModelDescription));
           this.Model = (fmiModelDescription)mySerializer.Deserialize(reader);*/

        #region FIELDS

     

        //DATA
        FMUVariables<int> _i = new FMUVariables<int>();
        FMUVariables<bool> _b = new FMUVariables<bool>();
        FMUVariables<double> _r = new FMUVariables<double>();
        FMUVariables<string> _s = new FMUVariables<string>();

        #endregion


        #region PROPERTIES
        protected override List<double> R
        {
            get {
                
                return _r.Value; }
        }

        protected override List<int> I
        {
            get { return _i.Value; }
        }

        protected override List<bool> B
        {
            get { return _b.Value; }
        }


        protected override List<string> S
        {
            get { return _s.Value; }
        }



        protected override int NUMBER_OF_STATES
        {
            get { return 1; }
        }

        protected override int NUMBER_OF_EVENT_INDICATORS
        {
            get { return 0; }
        }

        #endregion PROPERTIES DATA


        #region Values DATA
        string[] month = new string[] { "jan", "feb", "march", "april", "may", "june", "july", "august", "sept", "october", "november", "december" };
        const int x_ = 0;
        const int der_x_ = 1;
        #endregion

        #region Constructeur
        public FMU_Value()
        {
            try
            {
                //IF (_GUID != _DataModel.guid)
                //    throw new Exception("GUID : le modele de données ne correspond au FMU");            

                _r.Add("x_", 0d);
                _r.Add("der_x_", 0d);
                _i.Add("int_in_", 0);         //index 0
                _i.Add("int_out_", 0);        //index 1
                _b.Add("bool_in_", false);   //index 0
                _b.Add("bool_out_", false);     //index 1
                _s.Add("string_in_", "");    //index 0
                _s.Add("string_out_", ""); //index 1

                //Array states
                
                _vrStates = new int[NUMBER_OF_STATES];
                _vrStates[0]=_r.getIndex("x_");
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "FMU_Value: " + ee.Message);
            }

        }
        #endregion

        #region Body

        protected override double getReal(int index)
        {
            try
            {
                switch (index)
                {
                    case x_: return _r[x_];
                    case der_x_: return -_r[x_];
                    default: return 0;
                }
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "getReal: " + ee.Message);
                return 0;
            }
        }

        protected override void calculateValues()
        {
            try { 
            if (_state == fmi2ModelState.modelInitializationMode)
            {
                // set first time event
                _eventInfo.nextEventTimeDefined = true;
                _eventInfo.nextEventTime = 1 + _time;
            }
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "calculateValues: " + ee.Message);
            }
        }

        protected override void setStartValues()
        {
            try { 
            _r["x_"] = 1;
            _i["int_in_"] = 2;
            _i["int_out_"] = 0;
            _b["bool_in_"] = true;
            _b["bool_out_"] = false;
            _s["string_in_"] = "CSTB";
            _s["string_out_"] = month[0];
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "setStartValues: " + ee.Message);
            }
        }

        protected override void eventUpdate(bool isTimeEvent)
        {
            try
            {
                if (isTimeEvent)
                {
                    _eventInfo.nextEventTimeDefined = true;
                    _eventInfo.nextEventTime = 1 + _time;
                    _i["int_out_"] += 1;
                    _b["bool_out_"] = !_b["bool_out_"];
                    if (_i["int_out_"] < 12) _s["string_out_"] = month[_i["int_out_"]];
                    else _eventInfo.terminateSimulation = true;

                    //FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.INFO,
                    //    string.Format("eventUpdate: nextEventTime {0}", _eventInfo.nextEventTime.ToString()));
                }
            }
            catch (Exception ee)
            {

                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, "eventUpdate: " + ee.Message);
            }
        }

        protected override double getEventIndicator(int z)
        {
            return 0;
        }
        #endregion
    }
}
