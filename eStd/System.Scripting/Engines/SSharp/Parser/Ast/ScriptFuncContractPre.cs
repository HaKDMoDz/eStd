/*
 * Copyright � 2011, Petro Protsyk, Denys Vuika
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

using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptFuncContractPre : ScriptExpr
  {
    private readonly ScriptExprList _list;

    public ScriptFuncContractPre(AstNodeArgs args)
      : base(args)
    {
      _list = ChildNodes[1] as ScriptExprList;
    }

    public override void Evaluate(IScriptContext context)
    {
      bool result = true;

      if (_list == null)
      {
        context.Result = true;
        return;
      }

      _list.Evaluate(context);
      var rez = (object[])context.Result;

      foreach (var o in rez)
      {
        try
        {
          result = result & (bool)o;
        }
        catch
        {
          throw new ScriptVerificationException(Strings.VerificationNonBoolean);
        }
      }

      context.Result = result;
    }
  }
}