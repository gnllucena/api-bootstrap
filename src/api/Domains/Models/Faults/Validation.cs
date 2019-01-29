using System.Runtime.Serialization;

namespace API.Domains.Models.Faults
{
    public enum Validation
    {
        PaginationExceedsLimits = 1,
        UserNotInformed = 2,
        UserProfileNotInformed = 3,
        UserCountryNotInfored = 4,
        UserNameNotInformed = 5,
        UserNameExceedsLimit = 6,
        UserEmailNotInformed = 7,
        UserEmailExceedsLimit = 8,
        UserEmailNotValid = 9,
        UserDocumentNotInformed = 10,
        UserDocumentInvalid = 11,
        UserBirthdateNotInformed = 12,
        UserBirthdateInvalid = 13,
        UserRepeatedDocument = 14,
        UserRepeatedEmail = 15
    }
}
