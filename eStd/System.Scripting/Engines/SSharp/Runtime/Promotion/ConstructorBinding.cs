﻿/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Reflection;

namespace Scripting.SSharp.Runtime.Promotion
{
  internal class ConstructorBinding : IBinding
  {
    ConstructorInfo Method { get; set; }
    object[] Arguments { get; set; }

    public Type Type { get; private set; }

    public ConstructorBinding(ConstructorInfo method, object[] arguments)
    {
      Method = method;
      Arguments = arguments;
      Type = method.DeclaringType;
    }

    public ConstructorBinding(Type type, ConstructorInfo method, object[] arguments)
    {
      Method = method;
      Arguments = arguments;
      Type = type;
    }
    #region IInvokable Members

    public bool CanInvoke()
    {
      return Method != null;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      return Method.Invoke(Arguments);
    }

    #endregion
  }
}
