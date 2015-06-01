using System;
using System.Collections.Generic;
using System.Database.Database;
using System.Database.Document;
using System.Database.Utils;
using System.Linq;
using System.Text;

namespace LiteDB.Shell
{
    public interface ILiteCommand
    {
        bool IsCommand(StringScanner s);
        BsonValue Execute(LiteDatabase db, StringScanner s);
    }
}
