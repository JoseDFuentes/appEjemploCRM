using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel.Description;


namespace appEjemploCRM
{
    class Conexion
    {
        public string serviceUrl { get; set; }
        public string user { get; set; }
        public string password { get; set; }

        protected virtual ClientCredentials getCredentials()
        {
            ClientCredentials credential = new ClientCredentials();
            credential.UserName.UserName = this.user;
            credential.UserName.Password = this.password;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            return credential;
        }

        public OrganizationServiceProxy initServiceProxy()
        {
            OrganizationServiceProxy service;
            ClientCredentials clientCredentials = this.getCredentials();
            Uri serviceUri = new Uri(this.serviceUrl);

            service = new OrganizationServiceProxy(serviceUri, null, clientCredentials, null);
            service.Timeout = new TimeSpan(0, 10, 0);

            return service;
        }


    }
}
