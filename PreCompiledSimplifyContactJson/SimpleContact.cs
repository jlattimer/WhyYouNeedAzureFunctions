using System;

namespace PreCompiledSimplifyContactJson
{
    public class SimpleContact
    {
        public Guid CrmId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address1_Line1 { get; set; }
        public string Address1_City { get; set; }
        public string Address1_State { get; set; }
        public string Address1_Zip { get; set; }
    }
}
