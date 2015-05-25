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

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Base interface for Script Context obect.
  /// ScriptContext object stores run-time information during script's execution
  /// This information containts:
  ///    Scopes - which stores variables, types and functions
  ///    Execution Flow Flags - break, return, continue
  ///    
  /// ScriptContext objects also evaluates operators
  /// </summary>
  public interface IScriptContext : IDisposable
  {
    #region Scopes
    /// <summary>
    /// Create scope
    /// </summary>
    IScriptScope CreateScope();
    /// <summary>
    /// Add given scope to hierarchy
    /// </summary>
    /// <param name="scope">new scope</param>
    IScriptScope CreateScope(IScriptScope scope);
    /// <summary>
    /// Removes local scope
    /// </summary>
    void RemoveLocalScope();
    /// <summary>
    /// Current scope
    /// </summary>
    IScriptScope Scope { get; }

    /// <summary>
    /// Returns item from scope hierarchy
    /// </summary>
    /// <param name="id">name</param>
    /// <param name="throwException"></param>
    /// <returns>value</returns>
    object GetItem(string id, bool throwException);

    /// <summary>
    /// Sets item to scope hierarchy
    /// </summary>
    /// <param name="id">name</param>
    /// <param name="value">value</param>
    void SetItem(string id, object value);

    /// <summary>
    /// Create reference to an exising variable in scope hierarchy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IValueReference Ref(string id);
  
    /// <summary>
    /// Occurs before referencing process, giving ability to cancel it and return custom result
    /// </summary>
    event EventHandler<ReferencingEventArgs> Referencing;

    /// <summary>
    /// Occurs when referencing successful just before returning value
    /// </summary>
    event EventHandler<ReferencingEventArgs> Referenced;

    /// <summary>
    /// Finds function definition
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>function object</returns>
    IInvokable GetFunctionDefinition(string name);
    #endregion

    #region Break-Continue-Return
    void SetReturn(bool val);
    void SetBreak(bool val);
    void SetContinue(bool val);
    
    bool IsReturn();
    bool IsBreak();
    bool IsContinue();

    /// <summary>
    /// Reset all flags that control execution. Called on each context 
    /// before and after script execution
    /// </summary>
    void ResetControlFlags();
    #endregion

    #region Common
    /// <summary>
    /// Result of script execution
    /// </summary>
    object Result
    {
      get;
      set;
    }

    Script Owner
    {
      get;
    }
    #endregion
  }
}
