using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Shared.Exceptions
{
    public sealed class ObjectNotFoundException : NotFoundException
    {
        public ObjectNotFoundException(Guid objectId) : base($"The object with the identifier {objectId} was not found.")
        {
        }
    }
}
