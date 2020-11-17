using System;

namespace Common.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public bool Confirmed { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} - Name: {Name} - Email: {Email} - Password: {Password} - Active: {Active} - Confirmed: {Confirmed} - Created: {Created} - CreatedBy: {CreatedBy} - Updated: {Updated} - UpdatedBy: {UpdatedBy}";
        }
    }
}
