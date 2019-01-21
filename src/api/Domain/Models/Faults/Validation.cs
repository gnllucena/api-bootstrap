using System.Runtime.Serialization;

namespace API.Domain.Models.Faults
{
    public enum Validation
    {
        UserNotInformed = 1,
        UserProfileNotInformed = 2,
        UserCountryNotInfored = 3,
        UserNameNotInformed = 4,
        UserNameExceedsLimit = 5,
        UserEmailNotInformed = 6,
        UserEmailExceedsLimit = 7,
        UserEmailNotValid = 8,
        UserDocumentNotInformed = 9,
        UserDocumentInvalid = 10,
        UserBirthdateNotInformed = 11,
        UserBirthdateInvalid = 12
    }
}
