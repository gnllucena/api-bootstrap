using System.Runtime.Serialization;

namespace API.Domains.Queries
{
    public static class UserQuery
    {
        public const string GET = @"
            ";

        public const string PAGINATE = @"";
        
        public const string INSERT = @"";
        
        public const string UPDATE = @"";

        public const string DELETE = @"";

        public const string ACTIVATE_DEACTIVATE = @"";

        public const string EXISTS_EMAIL = @"
            SELECT count(1) 
              FROM bootstrap.User
             WHERE Email = :Email
        ";

        public const string EXISTS_DOCUMENT = @"
            SELECT count(1) 
              FROM bootstrap.User
             WHERE Documento = :Document
        ";
    }
}
