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

using System;
using System.Linq;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Script Array Expression List Expression
  /// </summary>
  internal class ScriptTypeExprList : ScriptExpr
  {
    internal ScriptTypeExpr[] ExprList
    {
      get
      {
        return ChildNodes.OfType<ScriptTypeExpr>().ToArray();
      }
    }
  
    public ScriptTypeExprList(AstNodeArgs args)
        : base(args)
    {
    }

    public override void Evaluate(IScriptContext context)
    {
      var listObjects = new Type[ExprList.Length];
      for (var i = 0; i < ExprList.Length; i++)
      {
        ExprList[i].Evaluate(context);
        listObjects[i] = (Type)context.Result;
      }
      context.Result = listObjects;
    }
  }
}
