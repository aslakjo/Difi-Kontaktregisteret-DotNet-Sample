using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace KontaktregisteretGateway
{
    public class MyIdentityVerifier : IdentityVerifier
    {
        private readonly IdentityVerifier _defaultVerifier;

        public MyIdentityVerifier()
        {
            _defaultVerifier = CreateDefault();
        }
        
        public override bool CheckAccess(EndpointIdentity identity,
            AuthorizationContext authContext)
        {
            // The following implementation is for demonstration only, and
            // does not perform any checks regarding EndpointIdentity.
            // Do not use this for production code.
            return true;
        }

        public override bool TryGetIdentity(EndpointAddress reference,
            out EndpointIdentity identity)
        {
            return _defaultVerifier.TryGetIdentity(reference, out identity);
        }
    }

}
