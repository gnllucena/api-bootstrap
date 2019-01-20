using System.Runtime.Serialization;

namespace API.Domain.Models
{
    public enum Profile
    {
        [EnumMember(Value = "Administrator")]
        Administrator = 1,

        [EnumMember(Value = "Regular")]
        Regular = 2
    }
}