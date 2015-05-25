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

using Scripting.SSharp.Parser.FastGrammar;

namespace Scripting.SSharp.Parser
{
  internal class SyntaxError
  {
    public SyntaxError(SourceLocation location, string message, ParserState state)
    {
      Location = location;
      Message = message;
      State = state;
    }

    public readonly SourceLocation Location;
    public readonly string Message;
    public readonly ParserState State;
  }
}
