using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PreCompiledSimplifyContactJson
{
    public class SimplifyContactJson
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req)
        {
            string content = req.Content.ReadAsStringAsync().Result;
            ContactContext k = JsonConvert.DeserializeObject<ContactContext>(content);

            SimpleContact contact = new SimpleContact
            {
                CrmId = new Guid(GetAttribute(k, "contactid").ToString()),
                FirstName = GetAttribute(k, "firstname")?.ToString(),
                LastName = GetAttribute(k, "lastname")?.ToString(),
                EmailAddress = GetAttribute(k, "emailaddress1")?.ToString(),
                Address1_Line1 = GetAttribute(k, "address1_line1")?.ToString(),
                Address1_City = GetAttribute(k, "address1_city")?.ToString(),
                Address1_State = GetAttribute(k, "address1_stateorprovince")?.ToString(),
                Address1_Zip = GetAttribute(k, "address1_postalcode")?.ToString()
            };

            HttpResponseMessage response =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(contact))
                };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        private static object GetAttribute(ContactContext k, string name)
        {
            Attribute attr = k.InputParameters[0].value.Attributes.FirstOrDefault(a => a.key == name);
            return attr?.value;
        }
    }
}
