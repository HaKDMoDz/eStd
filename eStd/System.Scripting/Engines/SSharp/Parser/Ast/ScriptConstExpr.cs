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
  /// Constant Expression
  /// </summary>
  internal class ScriptConstExpr : ScriptExpr
  {
    /// <summary>
    /// Value of the constant
    /// </summary>
    public object Value { get; set; }

    protected internal override bool IsConst {
        get {
            return true;
        }
    }

    public ScriptConstExpr(AstNodeArgs args)
        : base(args)
    {
      TokenAst constant = (TokenAst)ChildNodes[0];
      Value = constant.Value;

      if (constant.IsKeyword)
      {
        if (Value.Equals("true")) Value = true;
        else if (Value.Equals("false")) Value = false;
        else if (Value.Equals("null")) Value = null;
      }
    }

    public override void Evaluate(IScriptContext context)
    {
      context.Result = Value;
    }
  }
}