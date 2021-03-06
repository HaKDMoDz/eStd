﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Database.Database;
using System.Database.Document;
using System.Database.Utils;
using System.Linq;
using System.Text;

namespace LiteDB.Shell.Commands
{
    internal class Info : ILiteCommand
    {
        public bool IsCommand(StringScanner s)
        {
            return s.Match(@"db\.info$");
        }

        public BsonValue Execute(LiteDatabase db, StringScanner s)
        {
            return db.GetDatabaseInfo();
        }
    }
}
