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
using System.Text;

namespace Scripting.SSharp.Runtime.Promotion
{
  /// <summary>
  /// Represents implementation of Expando object via IScriptable interface.
  /// By default Expando objects created with a following syntax construct:
  ///   [name->valueExpression, name->valueExpression];
  /// This default type may be overriden by ScriptableObjectType option
  /// in script configuration.
  /// </summary>
  public class Expando : IScriptable, ISupportAssign
  {
    #region Fields

    readonly Dictionary<string, object> _fields = new Dictionary<string, object>();
    #endregion

    #region Public Members
    public IEnumerable<string> Fields
    {
      get
      {
        return _fields.Keys;
      }
    }

    public void AssignTo(object target)
    {
      var scope = target as IScriptScope;
      if (scope != null)
      {
        foreach (string field in Fields)
          scope.SetItem(field, _fields[field]);

        return;
      }

      foreach (string field in Fields)
      {
        IMemberBinding bind = RuntimeHost.Binder.BindToMember(target, field, false);
        if (bind != null)
          bind.SetValue(_fields[field]);
        //RuntimeHost.Binder.Set(field, target, fields[field], false);
      }
    }

    public object this[string fieldName]
    {
      get
      {
        object result;
        if (_fields.TryGetValue(fieldName, out result)) return result;

        return null;
      }
    }

    public override string ToString()
    {
      bool first = true;
      var builder = new StringBuilder("[");
      foreach (var field in Fields)
      {
        if (!first) builder.Append(','); else first = false;

        builder.Append(field);
        builder.Append("->");

        var value = this[field];
        builder.Append(value == null ? "null" : value.ToString());
      }
      builder.Append("]");
      return builder.ToString();
    }
    #endregion

    #region IScriptable
    [Promote(false)]
    public virtual object Instance
    {
      get { return this; }
    }

    [Promote(false)]
    public virtual IMemberBinding GetMember(string name, params object[] arguments)
    {
      if (arguments != null && arguments.Length != 0) return null;

      return new ExpandoBind(this, name);
    }

    [Promote(false)]
    public virtual IBinding GetMethod(string name, params object[] arguments)
    {
      if (arguments != null && arguments.Length != 0) return null;
      if (!_fields.ContainsKey(name)) throw new ScriptMethodNotFoundException(name);
      return new ExpandoBind(this, name);
    }

    #endregion

    #region ExpandoBind
    protected class ExpandoBind : IMemberBinding
    {
      private readonly Expando _expando;
      private readonly string _name;

      public ExpandoBind(Expando expando, string name)
      {
        _expando = expando;
        _name = name;
      }

      #region IMemberBind

      public object Target
      {
        get { return _expando; }
      }

      public Type TargetType
      {
        get { return typeof(object); }
      }

      public System.Reflection.MemberInfo Member
      {
        get { throw new NotSupportedException(); }
      }

      public void SetValue(object value)
      {
        if (_expando._fields.ContainsKey(_name))
        {
          _expando._fields[_name] = value;
        }
        else
        {
          _expando._fields.Add(_name, value);
        }
      }

      public object GetValue()
      {
        object result;
        if (_expando._fields.TryGetValue(_name, out result))
          return result;

        throw new ScriptIdNotFoundException(_name);
      }

      public void AddHandler(object value)
      {
        throw new NotSupportedException();
      }

      public void RemoveHandler(object value)
      {
        throw new NotSupportedException();
      }

      #endregion

      #region IInvokable

      public bool CanInvoke()
      {
        return true;
      }

      public object Invoke(IScriptContext context, object[] args)
      {
        var method = GetValue() as IInvokable;
        if (method != null)
          return method.Invoke(context, args);

        throw new ScriptIdNotFoundException(string.Format(Strings.MethodNotFound, _name));
      }

      #endregion
    }
    #endregion
  }
}
