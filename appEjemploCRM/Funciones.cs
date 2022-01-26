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
    class Funciones
    {
        OrganizationServiceProxy serviceProxy;
        Conexion conexion = new Conexion();

        public void initConection()
        {
            conexion.serviceUrl = "https://lccorpcrdesa.api.crm.dynamics.com/XRMServices/2011/Organization.svc";
            conexion.user = "apalavicini@lccorpcr.onmicrosoft.com";
            conexion.password = "Wut61666";
            serviceProxy = conexion.initServiceProxy();

        }

        public string getContactGuid()
        {
            string ret = "";

            QueryExpression queryContact = new QueryExpression();
            queryContact.EntityName = "contact";
            queryContact.ColumnSet = new ColumnSet(true);

            
    

            Entity entity = new Entity("contact");


            EntityCollection entities = serviceProxy.RetrieveMultiple(queryContact);

            if (entities.Entities.Count > 0)
            {
                entity = entities[0];
                ret = entity.Id.ToString();
            }


            return ret;
        }

        public string getContactByName(string _name)
        {
            string ret = string.Empty;

            QueryExpression queryContact = new QueryExpression()
            {
                EntityName = "contact",
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("firstname", ConditionOperator.Equal, _name)
                            }
                        }
                    }
                }

            };

            EntityCollection entities = serviceProxy.RetrieveMultiple(queryContact);
            
            if (entities.Entities.Count == 0)
            {
                return "Not Found";
            }

            string firstName = string.Empty;
            string lastname = string.Empty;
            string fullname = string.Empty;
            string familyStatus = string.Empty;
            string fax = string.Empty;
            string output = string.Empty;
            string lead = string.Empty;
            string creditLimitString = string.Empty;
            bool creditOnHold = false;
            string creditOnHoldString = string.Empty;
            DateTime birthDate = new DateTime();
            string birthDateString = string.Empty;

            int familyStatusCodeValue = 0;

            foreach (Entity contact in entities.Entities)
            {
                firstName = contact.Attributes["firstname"].ToString();
                lastname = contact["lastname"].ToString();
                fullname = contact["fullname"].ToString();
                if (contact.Attributes.Contains("fax"))
                {
                    fax = contact["fax"].ToString();
                }
                else
                {
                    fax = "Fax not found";
                }

                if (contact.Attributes.Contains("familystatuscode"))
                {
                    familyStatusCodeValue = ((OptionSetValue)contact["familystatuscode"]).Value;
                    familyStatus = this.familyStatus(familyStatusCodeValue);
                }

                if (contact.Attributes.Contains("originatingleadid"))
                {
                    EntityReference originatingLead = (EntityReference)contact["originatingleadid"];
                    lead = this.getLeadInfo(originatingLead);
                }

                if (contact.Attributes.Contains("creditlimit"))
                {
                    Money creditLimit = new Money();
                    creditLimit = (Money)contact["creditlimit"];
                    EntityReference currencyReference = (EntityReference)contact["transactioncurrencyid"];

                    creditLimitString = string.Format("LIMITE CREDITO = [{0} {1}]", this.getCurrencyCode(currencyReference),
                        creditLimit.Value.ToString("#,##0.00"));

                }
                else
                {
                    EntityReference currencyReference = (EntityReference)contact["transactioncurrencyid"];
                    creditLimitString = string.Format("LIMITE CREDITO = [{0} 0.00]", this.getCurrencyCode(currencyReference)); 
                }

                if (contact.Attributes.Contains("creditonhold"))
                {
                    creditOnHold = bool.Parse(contact["creditonhold"].ToString());
                    creditOnHoldString = creditOnHold ? "Sí" : "No";
                    creditOnHoldString = string.Format("SUSPENSION DE CREDITO: {0}", creditOnHoldString);
                }


                birthDateString = string.Empty;
                if (contact.Attributes.Contains("birthdate"))
                {
                    birthDate = (DateTime)contact["birthdate"];
                    birthDateString = birthDate.ToShortDateString();
                }

                output = string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8}", fullname, firstName, lastname, fax, familyStatus, lead, creditLimitString, creditOnHoldString, birthDateString);
                Console.WriteLine(output);

                //originatingleadid
                //lead

            }

            return ret;
        }

        private string familyStatus(int _familyStatusCode)
        {
            string ret = string.Empty;

            switch (_familyStatusCode)
            {
                case 1: ret = "Soltero/a"; break;
                case 2: ret = "Casado/a"; break;
                case 3: ret = "Divorciado/a"; break;
                case 4: ret = "Viudo/a"; break;
                default: ret = "No definido"; break;
            }
            return ret;

        }

        private string getLeadInfo(EntityReference _leadReference)
        {
            string ret = string.Empty;
            Entity lead = new Entity();
            string leadName = string.Empty;
            string leadLastName = string.Empty;
            lead = serviceProxy.Retrieve("lead", _leadReference.Id, new ColumnSet("firstname", "lastname"));
            if (lead.Attributes.Contains("firstname"))
            {
                leadName = lead["firstname"].ToString();
            }
            if (lead.Attributes.Contains("lastname"))
            {
                leadLastName = lead["lastname"].ToString();
            }

            ret = string.Format("CLIENTE POTENCIAL ORIGINAL = [{0} {1}]", leadName, leadLastName);

            return ret;
        }

        private string getCurrencyCode(EntityReference _currencyReference)
        {
            string ret = string.Empty;
            Entity currency = new Entity();
            
            currency = serviceProxy.Retrieve("transactioncurrency", _currencyReference.Id, new ColumnSet("currencysymbol"));
            if (currency.Attributes.Contains("currencysymbol"))
            {
                ret = currency["currencysymbol"].ToString();  
            }

            return ret;
        }

        public string createUpdateContact(Contact _contact)
        {
            Entity contact = new Entity("contact");
            string ret = string.Empty;
            bool exist = false;

            if (_contact.GUID != null)
            {
                exist = true;
                Guid guidContact = new Guid(_contact.GUID);
                contact = serviceProxy.Retrieve("contact", guidContact, new ColumnSet(true));
            }

            contact["firstname"] = _contact.firstName;
            contact["lastname"] = _contact.lastName;
            contact["fax"] = _contact.fax;
            contact["emailaddress1"] = _contact.email;
            contact["telephone1"] = _contact.jobphone;

            //campo grupo de opciones
            contact["familystatuscode"] = new OptionSetValue(_contact.familystatuscode);

            //campo money
            contact["creditlimit"] = new Money(_contact.creditlimit);

            //dós opciones
            contact["creditonhold"] = _contact.creditonhold;

            //campos fechas
            contact["birthdate"] = _contact.birthdate;



            try
            {
                if (!exist)
                {
                    ret = serviceProxy.Create(contact).ToString();
                    ret = string.Format("Contacto creado: {0}", ret);
                }
                else
                {
                    serviceProxy.Update(contact);
                    ret = string.Format("Contacto actualizado {0}", _contact.GUID);
                }
                
            }
            catch(Exception ex)
            {
                ret = string.Format("Error: {0}", ex.Message);
            }

            return ret;

        }

    }
}
