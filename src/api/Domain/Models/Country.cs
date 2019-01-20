using System.Runtime.Serialization;

namespace API.Domain.Models
{
    public enum Country
    {
        [EnumMember(Value = "Kanto")]
        Kanto = 1,

        [EnumMember(Value = "Johto")]
        Johto = 2,

        [EnumMember(Value = "Hoenn")]
        Hoenn = 3,

        [EnumMember(Value = "Sinnoh")]
        Sinnoh = 4,

        [EnumMember(Value = "Unova")]
        Unova = 5,

        [EnumMember(Value = "Kalos")]
        Kalos = 6,

        [EnumMember(Value = "Alola")]
        Alola = 7,
    }
}
