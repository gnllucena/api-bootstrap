using System.Runtime.Serialization;

namespace API.Domains.Models
{
    public enum Profile
    {
        [EnumMember(Value = "Administrator")]
        Administrator = 1,

        [EnumMember(Value = "Regular")]
        Regular = 2
    }
}