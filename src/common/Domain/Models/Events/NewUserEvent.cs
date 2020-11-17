using Common.Domain.Entities;

namespace Common.Domain.Models.Events
{
    public class NewUserEvent
    {
        public NewUserEvent(User user)
        {
            Id = user.Id;
        }

        public int Id { get; set; }
    }
}
