using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using KontaktregisteretGateway.Difi;

namespace KontaktregisteretGateway
{
    public class Gateway
    {
        public HentPersonerResponse Execute(string[] personalNumber)
        {
            var serviceEndpoint = new EndpointAddress(new Uri("https://kontaktinfo-ws-ver2.difi.no/kontaktinfo-external/ws-v3"));

            var binding = new CustomBinding();

            var securityBinding =
                  (AsymmetricSecurityBindingElement)SecurityBindingElement.CreateMutualCertificateBindingElement(
                          MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10);

            securityBinding.LocalClientSettings.IdentityVerifier = new MyIdentityVerifier();
            securityBinding.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic128Rsa15;

            securityBinding.IncludeTimestamp = true;
            securityBinding.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            securityBinding.EnableUnsecuredResponse = true;
            
            binding.Elements.Add(securityBinding);
            
            binding.Elements.Add(new TextMessageEncodingBindingElement() { MessageVersion = MessageVersion.Soap12 });
            binding.Elements.Add(new HttpsTransportBindingElement());
            
            using (var factory =
                new ChannelFactory<oppslagstjeneste1405Channel>(binding, serviceEndpoint))
            {
                var credentials = new MyClientCredentials();
                SetupCertificates(credentials);
                factory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
                factory.Endpoint.Behaviors.Add(credentials);

                var channel = factory.CreateChannel();
                try
                {
                    var forespoersel = new HentPersonerForespoersel();
                    forespoersel.informasjonsbehov = new informasjonsbehov[1];
                    forespoersel.informasjonsbehov[0] = informasjonsbehov.Kontaktinfo;
                    forespoersel.personidentifikator = new string[personalNumber.Length];
                    for (var index = 0; index < personalNumber.Length; index++)
                    {
                        forespoersel.personidentifikator[index] = personalNumber[index];
                    }

                    var request = new HentPersonerRequest(forespoersel);
                    var response = channel.HentPersoner(request);
                    channel.Close();
                    return response;
                }
                catch
                {
                    channel.Abort();
                    throw;
                }
            }
        }

        private static void SetupCertificates(MyClientCredentials credentials)
        {
            credentials.ClientEncryptingCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("Certificates\\idporten-ver2.difi.no-v2.crt", "changeit");
            credentials.ClientSigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("Certificates\\WcfClient.pfx", "changeit");

            credentials.ServiceEncryptingCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("Certificates\\WcfClient.pfx", "changeit");
            credentials.ServiceSigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("Certificates\\idporten-ver2.difi.no-v2.crt", "changeit");
        }
    }
}
