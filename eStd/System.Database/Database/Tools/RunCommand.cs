using System.Database.Document;
using System;
using System.Database.Shell;
using System.Linq;

namespace System.Database.Database
{
    public partial class LiteDatabase
    {
        private LiteShell _shell = null;

        /// <summary>
        /// Run a shell command in current database. Returns a BsonValue as result
        /// </summary>
        public BsonValue RunCommand(string command)
        {
            if (_shell == null)
            {
                _shell = new LiteShell(this);
            }
            return _shell.Run(command);
        }
    }
}
