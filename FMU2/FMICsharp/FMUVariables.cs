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
    public enum FMUVariableTypeEnum : int
    {
        IN = 0,
        OUT = 1,
        INOUT = 2,
    }
    public class FMUVariables<T>
    {
        public void Add(string aName, T value, FMUVariableTypeEnum aType = FMUVariableTypeEnum.INOUT)
        {
            if (exist(aName,true)) return ;
            _Name.Add(aName);
            _Value.Add(value);
            _TypeVariable.Add(aType);

        }

        
        private List<string> _Name = new List<string>();
        public List<string> Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<T> _Value = new List<T>();
        public List<T> Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private List<FMUVariableTypeEnum> _TypeVariable = new List<FMUVariableTypeEnum>();
        public List<FMUVariableTypeEnum> TypeVariable
        {
            get { return _TypeVariable; }
            set { _TypeVariable = value; }
        }
        public T this[int i]
        {
            get
            {
                return _Value[i];
            }
            set
            {
                _Value[i] = value;
            }
        }
        public T this[string aName]
        {
            get
            {
                exist(aName);
                return _Value[_Name.FindIndex(e => e == aName)];
            }
            set
            {
                exist(aName);               
                _Value[_Name.FindIndex(e => e == aName)] = value;

            }
        }
        private bool exist(string aName,bool flag=false)
        {
            if (flag && _Name.Exists(e=>e==aName))
            {
                FMICsharp.Log.Logger.WriteLog(FMICsharp.Log.LogLevelL4N.ERROR, string.Format("FMUVariables: unknown variable {0}", aName));
                return true;
            }
            return false;
        }

        public int getIndex(string aName) 
        {
            exist(aName);
            return _Name.FindIndex(e => e == aName);
        }
    }
}
