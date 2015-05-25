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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Scripting.SSharp.Diagnostics;
using Scripting.SSharp.Runtime.Configuration;

namespace Scripting.SSharp.Runtime
{
  public class BaseAssemblyManager : IAssemblyManager
  {
    #region Properties
    /// <summary>
    /// List of assemblies which are in use for the current moment.
    /// NOTE: This list may be changed during run-time (new assemblies may be added, some of them may be removed)
    /// </summary>
    protected  readonly List<Assembly> WorkingAssemblies = new List<Assembly>();

    /// <summary>
    /// Types cache. Contains all loaded types
    /// </summary>
    protected readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();

    /// <summary>
    /// Types by short names
    /// </summary>
    protected readonly Dictionary<string, Type> ShortTypes = new Dictionary<string, Type>();

    /// <summary>
    /// Cache of Namesapces
    /// </summary>
    protected readonly Dictionary<string, List<Type>> Namespaces = new Dictionary<string, List<Type>>();

    protected ScriptConfiguration Configuration { get; private set; }

    #endregion

    #region Initialization
    [Promote(false)]
    public BaseAssemblyManager()
    {
    }
    
    [Promote(false)]
    public virtual void Initialize(ScriptConfiguration configuration)
    {
      Requires.NotNull(configuration, "configuration");

      Configuration = configuration;
      FindAliasTypes();

      LoadAssemblies();
      ScanAssemblies();
    }
    #endregion

    #region Methods
    public virtual IEnumerable<MethodInfo> GetExtensionMethods(Type type) {
        return Enumerable.Empty<MethodInfo>();
    }

    public virtual IEnumerable<MethodInfo> GetExtensionMethods(Type type, string methodName) {
        return GetExtensionMethods(type).Where(m => m.Name == methodName);
    }
    /// <summary>
    /// Loads assemblies from configuration to memory and generate 
    /// LoadedAssemblies list which will be scanned for types
    /// </summary>
    protected virtual void LoadAssemblies()
    {
      if (Configuration == null) return;

      foreach (Reference reference in Configuration.References)
      {
        Assembly assembly = reference.Load();

        if (WorkingAssemblies.Contains(assembly))
          throw new NotSupportedException("Duplicate assembly in configuration");
        
        WorkingAssemblies.Add(assembly);
      }
    }

    /// <summary>
    /// Scans types in Loaded assemblies
    /// </summary>
    protected virtual void ScanAssemblies()
    {
      foreach (Assembly assembly in WorkingAssemblies.ToArray())
        AddAssembly(assembly);
    }

    protected virtual void RegisterType(Type type)
    {
      Requires.NotNull(type, "type");

      if (!Types.ContainsKey(type.FullName))
      {
        Types.Add(type.FullName, type);

        //Register in namespaces
        if (!string.IsNullOrEmpty(type.Namespace))
        {
          string[] parts = type.Namespace.Split('.');
          string current = null;
          List<Type> types = null;

          foreach (string part in parts)
          {
            if (current == null)
              current = part;
            else
              current += "." + part;

            if (!Namespaces.TryGetValue(current, out types))
            {
              types = new List<Type>();
              Namespaces.Add(current, types);
            }
          }

          // Add type to the last namespace
          types.Add(type);
        }
      }

      if (!ShortTypes.ContainsKey(type.Name))
      {
        ShortTypes.Add(type.Name, type);
      }
    }

    protected virtual void UnRegisterType(Type type)
    {
      Requires.NotNull(type, "type");

      if (Types.ContainsKey(type.FullName))
      {
        Types.Remove(type.FullName);

        //Clear namespace cache
        if (!string.IsNullOrEmpty(type.Namespace))
        {
          List<Type> types = Namespaces[type.Namespace];
          types.Remove(type);

          // Remove all empty subnamespaces
          string[] parts = type.Namespace.Split('.');
          string current = null;

          for (int i = 0; i < parts.Length; i++)
          {
            current = string.Join(".", parts.Take(parts.Length - i));

            types = Namespaces[current];
            if (types.Count == 0 && Namespaces.Keys.Where(p=>p.Contains(current+".")).Count()==0)
            {
              Namespaces.Remove(type.Namespace);
            }
          }
        }

      }

      if (ShortTypes.ContainsKey(type.Name))
      {
        ShortTypes.Remove(type.Name);
      }

      //Remove all aliases
      if (ShortTypes.ContainsValue(type))
      {
        var keysToRemove = (from value in ShortTypes where value.Value == type select value.Key).ToList();

        foreach (var key in keysToRemove)
          ShortTypes.Remove(key);
      }
    }

    private void FindAliasTypes()
    {
      if (Configuration == null) return;

      foreach (TypeXml typeXml in Configuration.Types)
      {
        Type type = RuntimeHost.GetNativeType(typeXml.QualifiedName);
        if (type == null) throw new NullReferenceException(string.Format("Type {0} is not found", typeXml.QualifiedName));
        AddType(typeXml.Alias, type);
      }
    }

    protected virtual AssemblyHandlerEventArgs OnBeforeAddAssembly(Assembly assembly)
    {
      Requires.NotNull(assembly, "assembly");

      var args = new AssemblyHandlerEventArgs(assembly);

      var hanlder = BeforeAddAssembly;
      if (hanlder != null) hanlder(this, args);

      return args;
    }

    public event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;

    protected virtual AssemblyTypeHandlerEventArgs OnBeforeAddType(Assembly assembly, Type type)
    {
      Requires.NotNull(assembly, "assembly");
      Requires.NotNull(type, "type");

      var args = new AssemblyTypeHandlerEventArgs(assembly, type);
      var handler = BeforeAddType;
      if (handler != null) handler(this, args);

      return args;
    }

    public event EventHandler<AssemblyTypeHandlerEventArgs> BeforeAddType;

    #endregion

    #region Public Interface
    public virtual void AddAssembly(Assembly assembly)
    {
      Requires.NotNull(assembly, "assembly");

      // Skip processing dynamic assemblies
      if (assembly.IsDynamic) return;

      if (OnBeforeAddAssembly(assembly).Cancel)
      {
        WorkingAssemblies.Remove(assembly);
        return;
      }

      if (!WorkingAssemblies.Contains(assembly))
      {
        WorkingAssemblies.Add(assembly);
      }

      foreach (Type type in assembly.GetExportedTypes())
      {
        if (!type.IsPublic) continue;

        if (OnBeforeAddType(assembly, type).Cancel)
          continue;

        RegisterType(type);
      }
    }

    public virtual void RemoveAssembly(Assembly assembly)
    {
      Requires.NotNull(assembly, "assembly");

      if (!WorkingAssemblies.Contains(assembly)) return;

      foreach (Type type in assembly.GetExportedTypes())
      {
        if (!type.IsPublic) continue;

        UnRegisterType(type);
      }

      WorkingAssemblies.Remove(assembly);
    }
    /// <summary>
    /// Returns type by given name
    /// </summary>
    /// <param name="name">Short, Alias or FullType name</param>
    /// <returns>Type</returns>
    /// <exception cref="ScriptIdNotFoundException">
    ///  If type not found
    /// </exception>
    public Type GetType(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      Type result;

      if (ShortTypes.TryGetValue(name, out result)) return result;
      if (Types.TryGetValue(name, out result)) return result;

      throw new ScriptIdNotFoundException(string.Format(Strings.TypeNotFoundError, name));
    }

    public bool HasType(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return ShortTypes.ContainsKey(name) || Types.ContainsKey(name);
    }

    /// <summary>
    /// Adds type to a manager
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="type"></param>
    public void AddType(string alias, Type type)
    {
      Requires.NotNullOrEmpty(alias, "alias");
      Requires.NotNull(type, "type");

      RuntimeHost.Lock();
      try
      {
        if (ShortTypes.ContainsKey(alias))
        {
          ShortTypes[alias] = type;
        }
        else
        {
          ShortTypes.Add(alias, type);
        }
      }
      finally
      {
        RuntimeHost.UnLock();
      }
    }

    public bool HasNamespace(string name)
    {
      Requires.NotNullOrEmpty(name, "name");

      return Namespaces.ContainsKey(name);
    }
    #endregion

    #region IDisposable Members
    [Promote(false)]
    public virtual void Dispose()
    {
      ShortTypes.Clear();
      Types.Clear();
    }

    #endregion
  }
}
