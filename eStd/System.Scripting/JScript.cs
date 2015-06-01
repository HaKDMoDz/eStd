using System;

namespace System.Scripting
{
    public class JScript : IScriptEngine
    {
        internal JScript()
        {

        }

        private Creek.Scripting.JScriptEngine _engine = new Creek.Scripting.JScriptEngine();

        public object InvokeFunction(string name, params object[] parameters)
        {
            return _engine.InvokeFunction(name, parameters);
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The obj.</param>
        public void AddObject(string name, object obj)
        {
            _engine.Add(name, obj);
        }

        /// <summary>
        /// Executes the specified SRC.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public void Execute(string src)
        {
            _engine.Execute(src);
        }

        /// <summary>
        /// Evals the specified SRC.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns></returns>
        public object Eval(string src)
        {
            return _engine.Evaluate(src);
        }
    }
}