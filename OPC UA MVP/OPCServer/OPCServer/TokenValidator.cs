using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCServer
{
    public interface ITokenValidator
    {
        IUserIdentity ValidateToken(IssuedIdentityToken issuedToken);
    }
}
