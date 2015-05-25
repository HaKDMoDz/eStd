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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptSwitchRootStatement : ScriptAst, IEnumerable<ScriptAst>
  {
    public ScriptExpr Expression { get { return (ScriptExpr)ChildNodes[1]; } }
    public ScriptSwitchStatement Switch { get { return (ScriptSwitchStatement)ChildNodes[2]; } }
    
    public ScriptSwitchRootStatement(AstNodeArgs args)
      : base(args)
    {

    }

    #region IEnumerable<ScriptAst> Members

    public IEnumerator<ScriptAst> GetEnumerator()
    {
      return ChildNodes.OfType<ScriptAst>().GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ChildNodes.GetEnumerator();
    }

    #endregion
  }
}
