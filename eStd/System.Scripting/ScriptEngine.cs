using System;

namespace System.Scripting
{
    public static class ScriptEngine
    {
        public static T Create<T>()
            where T : class, IScriptEngine
        {
            var t = typeof(T).Name;
            if (t == typeof(JScript).Name)
            {
                return new JScript() as T;
            }
            else if (t == typeof(VBScript).Name)
            {
                return new VBScript() as T;
            }
            else if (t == typeof(SSharp).Name)
            {
                return new SSharp() as T;
            }
            return default(T);
        }
    }
}