using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Database.Database;
using System.Database.Document;
using System.Database.Shell.Commands.Collections;
using System.Database.Utils;
using System.Linq;
using System.Text;

namespace LiteDB.Shell.Commands
{
    internal class CollectionDelete : BaseCollection, ILiteCommand
    {
        public bool IsCommand(StringScanner s)
        {
            return this.IsCollectionCommand(s, "delete");
        }

        public BsonValue Execute(LiteDatabase db, StringScanner s)
        {
            var col = this.ReadCollection(db, s);
            var query = this.ReadQuery(s);

            return col.Delete(query);
        }
    }
}
