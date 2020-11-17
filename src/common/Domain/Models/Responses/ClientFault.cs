using System.Collections.Generic;

namespace Common.Domain.Models.Responses
{
    public class ClientFault
    {
        public ClientFault()
        {
            Faults = new List<Fault>();
        }

        public string Message { get; set; }
        public IList<Fault> Faults { get; set; }
    }

    public class Fault
    {
        public string Error { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
