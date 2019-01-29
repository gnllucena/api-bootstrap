using System.Runtime.Serialization;

namespace API.Domains.Queries
{
    public static class UserQuery
    {
        public const string GET = @"
            SELECT Id,
                   IdProfile as Profile,
                   IdCountry as Country,
                   CreatedBy,
                   Name,
                   Email,
                   Document,
                   Birthdate,
                   Active
              FROM bootstrap.User
             WHERE Id = @Id
               AND CreatedBy = @CreatedBy;
        ";

        public const string PAGINATE = @"
            SELECT Id,
                   IdProfile as Profile,
                   IdCountry as Country,
                   CreatedBy,
                   Name,
                   Email,
                   Document,
                   Birthdate,
                   Active
              FROM bootstrap.User
             WHERE Id > @Offset
               AND CreatedBy = @CreatedBy
          ORDER BY ID ASC
             LIMIT @Limit;
        ";
        
        public const string TOTAL = @"
            SELECT COUNT(1)
              FROM bootstrap.User
             WHERE CreatedBy = @CreatedBy;
        ";

        public const string INSERT = @"
            INSERT INTO bootstrap.User 
                       (IdProfile,
                        IdCountry,
                        CreatedBy,
                        Name,
                        Email,
                        Document,
                        Birthdate,
                        Active)
                VALUES (@IdProfile,
                        @IdCountry,
                        @CreatedBy,
                        @Name,
                        @Email,
                        @Document,
                        @Birthdate,
                        @Active);

            SELECT LAST_INSERT_ID();                  
        ";
        
        public const string UPDATE = @"
            UPDATE bootstrap.User 
               SET IdProfile = @IdProfile,
                   IdCountry = @IdCountry,
                   Name = @Name,
                   Email = @Email,
                   Birthdate = @Birthdate,
                   Active = @Active
             WHERE Id = @Id;
        ";

        public const string DELETE = @"
            DELETE FROM bootstrap.User
                  WHERE Id = @Id
                    AND CreatedBy = @CreatedBy;
        ";

        public const string ACTIVATE_DEACTIVATE = @"
            UPDATE bootstrap.User 
               SET Active = @Active
             WHERE Id = @Id
               AND CreatedBy = @CreatedBy;
        ";

        public const string EXISTS_EMAIL = @"
            SELECT count(1) 
              FROM bootstrap.User
             WHERE Email = @Email;
        ";

        public const string EXISTS_SAME_EMAIL = @"
            SELECT count(1) 
              FROM bootstrap.User
             WHERE Email = @Email
               AND Id != @Id;
        ";

        public const string EXISTS_DOCUMENT = @"
            SELECT count(1) 
              FROM bootstrap.User
             WHERE Document = @Document;
        ";
    }
}
