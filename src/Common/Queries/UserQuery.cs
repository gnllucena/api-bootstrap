using Common.Domain.Entities;

namespace Common.Queries
{
    public class UserQuery
    {
        public static string INSERT = $@"
            INSERT INTO user
                       (
                        name,
                        email,
                        password,
                        active,
                        confirmed,
                        created,
                        createdby,
                        updated,
                        updatedby
                       )
                VALUES (
                        @name,
                        @email,
                        @password,
                        @active,
                        @confirmed,
                        @created,
                        @createdby,
                        @updated,
                        @updatedby
                       );

            SELECT LAST_INSERT_ID() as id
        ";

        public static string UPDATE = $@"
            UPDATE user SET
                   id        = @id,
                   name      = @name,
                   email     = @email,
                   password  = @password,
                   active    = @active,
                   confirmed = @confirmed,
                   created   = @created,
                   createdby = @createdby,
                   updated   = @updated,
                   updatedby = @updatedby
             WHERE id = @id
        ";

        public static string DELETE = $@"
            DELETE FROM user
                  WHERE id = @id
        ";

        public static string LIST = $@"
            SELECT 
                   id        as {nameof(User.Id)},
                   name      as {nameof(User.Name)},
                   email     as {nameof(User.Email)},
                   password  as {nameof(User.Password)},
                   active    as {nameof(User.Active)},
                   confirmed as {nameof(User.Confirmed)},
                   created   as {nameof(User.Created)},
                   createdby as {nameof(User.CreatedBy)},
                   updated   as {nameof(User.Updated)},
                   updatedby as {nameof(User.UpdatedBy)}
              FROM user
        ";

        public static string GET = $@"
            {LIST}
             WHERE id = @id
        ";

        public static string PAGINATE = $@"
            {LIST}
            {PAGINATE_WHERE}
             LIMIT @Limit
            OFFSET @Offset
        ";

        public static string PAGINATE_WHERE = $@"
            WHERE id = id
        ";

        public static string PAGINATE_COUNT = $@"
            SELECT COUNT(1)
              FROM user
            {PAGINATE_WHERE}
        ";

        public static string EXISTS_BY_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE id = @id
        ";

        public static string EXISTS_BY_NAME = $@"
            SELECT COUNT(1)
              FROM user
             WHERE name = @name
        ";

        public static string EXISTS_BY_EMAIL = $@"
            SELECT COUNT(1)
              FROM user
             WHERE email = @email
        ";

        public static string EXISTS_BY_PASSWORD = $@"
            SELECT COUNT(1)
              FROM user
             WHERE password = @password
        ";

        public static string EXISTS_BY_ACTIVE = $@"
            SELECT COUNT(1)
              FROM user
             WHERE active = @active
        ";

        public static string EXISTS_BY_CONFIRMED = $@"
            SELECT COUNT(1)
              FROM user
             WHERE confirmed = @confirmed
        ";

        public static string EXISTS_BY_CREATED = $@"
            SELECT COUNT(1)
              FROM user
             WHERE created = @created
        ";

        public static string EXISTS_BY_CREATEDBY = $@"
            SELECT COUNT(1)
              FROM user
             WHERE createdby = @createdby
        ";

        public static string EXISTS_BY_UPDATED = $@"
            SELECT COUNT(1)
              FROM user
             WHERE updated = @updated
        ";

        public static string EXISTS_BY_UPDATEDBY = $@"
            SELECT COUNT(1)
              FROM user
             WHERE updatedby = @updatedby
        ";

        public static string EXISTS_BY_NAME_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE name = @name
               AND id <> @id
        ";

        public static string EXISTS_BY_EMAIL_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE email = @email
               AND id <> @id
        ";

        public static string EXISTS_BY_PASSWORD_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE password = @password
               AND id <> @id
        ";

        public static string EXISTS_BY_ACTIVE_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE active = @active
               AND id <> @id
        ";

        public static string EXISTS_BY_CONFIRMED_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE confirmed = @confirmed
               AND id <> @id
        ";

        public static string EXISTS_BY_CREATED_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE created = @created
               AND id <> @id
        ";

        public static string EXISTS_BY_CREATEDBY_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE createdby = @createdby
               AND id <> @id
        ";

        public static string EXISTS_BY_UPDATED_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE updated = @updated
               AND id <> @id
        ";

        public static string EXISTS_BY_UPDATEDBY_AND_DIFFERENT_ID = $@"
            SELECT COUNT(1)
              FROM user
             WHERE updatedby = @updatedby
               AND id <> @id
        ";
    }
}
