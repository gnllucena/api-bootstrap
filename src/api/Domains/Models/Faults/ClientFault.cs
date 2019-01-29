using System.Collections.Generic;

namespace API.Domains.Models.Faults
{
    public class ClientFault 
    {
        public ClientFault() 
        {
            faults = new List<Fault>();
        }

        public string message { get; set; }
        public IList<Fault> faults { get; set; }

        public class Fault 
        {
            public string code { get; set; }
            public string error { get; set; }
            public string property { get; set; }
            public string value { get; set; }
        }
    }
}