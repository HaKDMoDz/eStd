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

#if SILVERLIGHT || PocketPC
using DefaultAssemblyManager = Scripting.SSharp.Runtime.BaseAssemblyManager;
#else
using DefaultAssemblyManager = Scripting.SSharp.Runtime.AssemblyManager;
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Scripting.SSharp.Runtime.Promotion;
using Scripting.SSharp.Runtime.Configuration;
using Scripting.SSharp.Runtime.Operators;
using Scripting.SSharp.Diagnostics;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Run-time configuration manager for Script.net
  /// </summary>
  public static class RuntimeHost
  {  
    #region Fields & Properties
    private static ScriptConfiguration _configuration;

    private static Script _initializationScript;

    /// <summary>
    /// Settings section
    /// </summary>
    private static readonly Dictionary<string, object> SettingsItems = new Dictionary<string, object>();

    /// <summary>
    /// Operators
    /// </summary>
    private static readonly Dictionary<string, IOperator> BinOperators = new Dictionary<string, IOperator>();

    /// <summary>
    /// Operators
    /// </summary>
    private static readonly Dictionary<string, IOperator> UnaryOperators = new Dictionary<string, IOperator>();

    /// <summary>
    /// Object used to synchronize during multi-threaded execution
    /// </summary>
    private static readonly object SyncRoot = new object();

    public static bool IsInitialized { get; private set; }

    [Promote(false)]
    public static IScopeFactory ScopeFactory { get; private set; }

    public static IObjectBinding Binder { get; set; }

    [Promote(false)]
    public static IAssemblyManager AssemblyManager { get; set; }

    /// <summary>
    /// Activator which used to activate instances
    /// </summary>
    public static IObjectActivator Activator { get; set; }

    private static readonly Dictionary<string, IOperatorHandler> Handlers = new Dictionary<string, IOperatorHandler>();

    /// <summary>
    /// Should be returned by GetVariableInternal if item with given
    /// name not found in the scope hierarchy
    /// </summary>
    public static object NoVariable = new object();

    public static object NullValue = new object();

    internal static Parser.FastGrammar.LRParser Parser { get; private set; }
    #endregion

    #region Construction & Initialization
    /// <summary>
    /// Load default configuration from RuntimeConfig.xml
    /// </summary>
    [Promote(false)]
    public static void Initialize()
    {
#if PocketPC || SILVERLIGHT
      Initialize(DefaultConfig);
#else
      Initialize(DefaultConfig);
      //Initialize(Configurations.CreateDefault());
#endif     
    }

    [Promote(false)]
    public static void Initialize(Stream configuration)
    {
      Initialize(LoadConfiguration(configuration));
    }

    /// <summary>
    /// Loads given configuration
    /// </summary>
    /// <param name="configuration"></param>
    [Promote(false)]
    public static void Initialize(ScriptConfiguration configuration)
    {
      if (IsInitialized) return;

      if (Parser == null)
      {
        Parser = new Parser.FastGrammar.LRParser();
      }

      Lock();
      try
      {
        _configuration = configuration;

        if (Binder == null)
          Binder = new DefaultObjectBinding();

        if (Activator == null)
          Activator = new ObjectActivator();

        InitializeSettingItems();
        RegisterOperators();

        if (ScopeFactory == null)
          ScopeFactory = Activator.CreateInstance(GetNativeType(GetSettingsItem<string>(ConfigSchema.ScopeFactoryAttribute))) as IScopeFactory;

        RegisterScopes();

        if (AssemblyManager == null)
          AssemblyManager = new DefaultAssemblyManager();

        OnInitializingTypes(AssemblyManager);
        AssemblyManager.Initialize(_configuration);

        if (!string.IsNullOrEmpty(_configuration.Initialization))
          _initializationScript = Script.Compile(_configuration.Initialization);

        RegisterOperatorHandler("+=", new EventOperatorHandler(true));
        RegisterOperatorHandler("-=", new EventOperatorHandler(false));

        SSharp.Parser.Ast.ScriptExpr.HandleOperator += HandleOperator;
      }
      finally
      {
        IsInitialized = true;
        UnLock();
      }
    }

    private static void RegisterOperators()
    {
      foreach (var definition in _configuration.Operators)
      {
        var oper = (IOperator)Activator.CreateInstance(GetNativeType(definition.Type));
        if (oper.Unary)
          UnaryOperators.Add(oper.Name, oper);
        else
          BinOperators.Add(oper.Name, oper);
      }
    }

    private static void HandleOperator(object sender, HandleOperatorArgs e)
    {
      IOperatorHandler handler;
      if (Handlers.TryGetValue(e.Symbol, out handler))
      {
        e.Result = handler.Process(e);
      }
    }

    private static void RegisterScopes()
    {
      //NOTE: Default values
      //ScopeFactory.RegisterType(ScopeTypes.Default, typeof(ScriptScope));
      //ScopeFactory.RegisterType(ScopeTypes.Contract, typeof(ScriptContractScope));
      //ScopeFactory.RegisterType(ScopeTypes.Using, typeof(ScriptUsingScope));

      foreach (ScopeDefinition definition in _configuration.Scopes)
      {
        ScopeFactory.RegisterType(definition.Id, (IScopeActivator)Activator.CreateInstance(GetNativeType(definition.Type)));
      }
    }

    /// <summary>
    /// Clears all information in the RuntimeHost
    /// </summary>
    [Promote(false)]
    public static void CleanUp()
    {
      Lock();
      try
      {
        SSharp.Parser.Ast.ScriptExpr.HandleOperator -= HandleOperator;
        Handlers.Clear();
        initializingTypes.Clear();
        Binder = null;
        Activator = null;
        ScopeFactory = null;

        if (AssemblyManager != null)
        {
          AssemblyManager.Dispose();
          AssemblyManager = null;
        }

        SettingsItems.Clear();
        BinOperators.Clear();
        UnaryOperators.Clear();
        _initializationScript = null;

        _configuration = new ScriptConfiguration();
      }
      finally
      {
        IsInitialized = false;
        UnLock();
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Loads language configuration from stream
    /// </summary>
    /// <param name="configStream"></param>
    private static ScriptConfiguration LoadConfiguration(Stream configStream)
    {
      var configurationSerializer = new XmlSerializer(typeof(ScriptConfiguration));
      var configuration = configurationSerializer.Deserialize(configStream) as ScriptConfiguration;
      if (configuration == null)
        throw new ScriptRuntimeException(Strings.WrongConfigurationError);

      return configuration;
    }
    #endregion

    #region Loading
    private static void InitializeSettingItems()
    {
      foreach (SettingXml item in _configuration.SettingXml)
      {
        object rez = item.Value;
        if (!string.IsNullOrEmpty(item.Converter))
          rez = GetItemValue(item.Value, item.Converter);

        SettingsItems.Add(item.Name, rez);
      }
    }

    private static object GetItemValue(string value, string converter)
    {
      Type converterType = GetNativeType(converter);
#if PocketPC || SILVERLIGHT
          try
          {
            return Convert.ChangeType(value, converterType, System.Globalization.CultureInfo.InvariantCulture);
          }
          catch
          {
            System.Diagnostics.Debug.WriteLine("Failed to convert string to type: " + converterType.ToString());
          }
#else
      var converterObject = Activator.CreateInstance(converterType) as TypeConverter;
      if (converterObject != null && converterObject.CanConvertFrom(typeof(string)))
      {
        return converterObject.ConvertFrom(value);
      }
#endif
      return value;
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Returns setting item specified in run-time config file
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static object GetSettingsItem(string id)
    {
      object result;
      if (SettingsItems.TryGetValue(id, out result))
        return result;

      return null;
    }

    public static void SetSettingItem(string id, object value)
    {
      Lock();
      try
      {
        if (SettingsItems.ContainsKey(id))
        {
          SettingsItems[id] = value;
        }
        else
        {
          SettingsItems.Add(id, value);
        }
      }
      finally
      {
        UnLock();
      }
    }
    /// <summary>
    /// Returns setting item specified in run-time config file
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T GetSettingsItem<T>(string id)
    {
      object result = GetSettingsItem(id);
      return result == null ? default(T) : (T)result;
    }

    [Promote(false)]
    public static IOperator GetBinaryOperator(string id)
    {
      IOperator oper;
      if (BinOperators.TryGetValue(id, out oper))
        return oper;

      throw new NotSupportedException(string.Format("Given operator {0} is not found", id));
    }

    [Promote(false)]
    public static IOperator GetUnaryOperator(string id)
    {
      IOperator oper;
      if (UnaryOperators.TryGetValue(id, out oper))
        return oper;

      throw new NotSupportedException(string.Format("Given operator {0} is not found", id));
    }

    [Promote(false)]
    public static void InitializeScript(IScriptContext context)
    {
      if (_initializationScript == null) return;

      Lock();
      _initializationScript.Context = context;
      _initializationScript.Execute();
      _initializationScript.Context = null;
      UnLock();
    }

    internal static Type GetNativeType(string name)
    {
      return Type.GetType(name);
    }

    public static Type GetType(string name)
    {
      return AssemblyManager.GetType(name);
    }

    public static bool HasType(string name)
    {
      return AssemblyManager.HasType(name);
    }

    [Promote(false)]
    public static void AddType(string alias, Type type)
    {
      Requires.NotNullOrEmpty(alias, alias);
      Requires.NotNull(type, "type");

      AssemblyManager.AddType(alias, type);
    }

    [Promote(false)]
    public static void AddType<T>(string alias)
    {
      Requires.NotNullOrEmpty(alias, "alias");

      AssemblyManager.AddType(alias, typeof(T));
    }

    [Promote(false)]
    public static void RegisterOperatorHandler(string operatorSymbol, IOperatorHandler handler)
    {
      if (!Handlers.ContainsKey(operatorSymbol))
        Handlers.Add(operatorSymbol, handler);
      else
        Handlers[operatorSymbol] = handler;
    }


    /// <summary>
    /// Lock's runtime host for threading execution 
    /// </summary>
    public static void Lock()
    {
      Monitor.Enter(SyncRoot);
    }

    /// <summary>
    /// Unlock's thread
    /// </summary>
    public static void UnLock()
    {
      Monitor.Exit(SyncRoot);
    }

    /// <summary>
    /// This event is raised before AssemblyManager starts creating type system.
    /// It should be used to subscribe on AssemblyManager's events in order to cancel
    /// loading some assemblies and adding particular types
    /// </summary>          
    public static event EventHandler<EventArgs> InitializingTypes
    {
      add
      {
        initializingTypes.Add(value);
      }
      remove
      {
        initializingTypes.Remove(value);
      }
    }

    private static readonly List<EventHandler<EventArgs>> initializingTypes = new List<EventHandler<EventArgs>>();

    private static void OnInitializingTypes(object sender)
    {
      foreach (var handler in initializingTypes)
        handler.Invoke(sender, EventArgs.Empty);
    }
    #endregion

    #region Config
    public static Stream DefaultConfig
    {
      get
      {
        var configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scripting.SSharp.RuntimeConfig.xml");
        configStream.Seek(0, SeekOrigin.Begin);
        return configStream;
      }
    }
    #endregion
  }
}
