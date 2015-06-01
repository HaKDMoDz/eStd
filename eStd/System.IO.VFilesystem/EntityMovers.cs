using System.IO.VFilesystem.Collections;
using System.Text;

namespace SharpFileSystem
{
    public static class EntityMovers
    {
        public static TypeCombinationDictionary<IEntityMover> Registration { get; private set; }

        static EntityMovers()
        {
            Registration = new TypeCombinationDictionary<IEntityMover>();
        }
    }
}
