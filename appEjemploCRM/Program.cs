using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appEjemploCRM
{
    class Program
    {
        static void Main(string[] args)
        {

            Funciones funcionesCRM = new Funciones();
            funcionesCRM.initConection();

            //string guidFirstEntity = funcionesCRM.getContactGuid();
            //string guidEntityByName = funcionesCRM.getContactByName("CESAR");

            Contact contact = new Contact();

            contact.GUID = "25d7d845-f2ef-eb11-bacb-000d3a546144";
            contact.firstName = "Juan Manuel";
            contact.lastName = "Mendez";
            contact.fax = "0000002";
            contact.familystatuscode = 1;
            contact.creditlimit = 500;
            contact.creditonhold = false;
            contact.birthdate = new DateTime(1970, 6, 15);
            contact.jobphone = "3333332";
            contact.email = "jmendez@mail.com";

            string res = funcionesCRM.createUpdateContact(contact);

            Console.WriteLine(res);

            Console.WriteLine(string.Format("Enter to exit"));
            Console.ReadLine();


        }
    }
}
