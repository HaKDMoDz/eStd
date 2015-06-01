using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Scripting
{
    public interface IScriptEngine
    {
        void AddObject(string name, object obj);
        void Execute(string src);
        object Eval(string src);
    }
}