using System.Security.Principal;
using FubuMVC.Core.Security;

namespace Scoring.Web.Security
{
    public class FubuPrincipalFactory : IPrincipalFactory
    {
        public IPrincipal CreatePrincipal(IIdentity identity)
        {
            return FubuPrincipal.Current
                ?? new FubuPrincipal(identity);
        }
    }
}