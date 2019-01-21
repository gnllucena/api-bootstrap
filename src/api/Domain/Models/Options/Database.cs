using System.Runtime.Serialization;

namespace API.Domain.Models.Options
{
    public class Database
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string Schema { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
