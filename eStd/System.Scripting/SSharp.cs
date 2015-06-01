using System;
using Scripting.SSharp;
using Scripting.SSharp.Runtime;

namespace System.Scripting
{
    public class SSharp : IScriptEngine
    {
        internal SSharp()
        {
            RuntimeHost.Initialize();
        }
 
        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The obj.</param>
        public void AddObject(string name, object obj)
        {
            if (obj is Type)
            {
                RuntimeHost.AddType(name, (Type)obj);
            }
            else
            {
                throw new ArgumentException("only Type accepted");
            }
        }

        /// <summary>
        /// Executes the specified SRC.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public void Execute(string src)
        {
            Script.Compile(src).Execute();
        }

        /// <summary>
        /// Evals the specified SRC.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns></returns>
        public object Eval(string src)
        {
            return Script.Compile(src).Execute();
        }
    }
}