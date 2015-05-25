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
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base implementation of IObjectActivator interface. Default ObjectActivator used
  /// by RuntimeHost to create any instance from the script.
  /// </summary>
  public class ObjectActivator : IObjectActivator
  {
    #region IObjectActivator Members
    public object CreateInstance(Type type, object[] args)
    {
      return CreateInstance(null, RuntimeHost.Binder.BindToConstructor(type, args));
    }

    public object CreateInstance(IScriptContext context, IBinding bind)
    {
      return bind.Invoke(context, null);
    }

    #endregion
  }
}
