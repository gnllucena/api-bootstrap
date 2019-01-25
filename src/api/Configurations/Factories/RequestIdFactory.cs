using System;

namespace API.Configurations.Factories
{
    public interface IRequestIdFactory
    {
        string Id();
    }

    public class RequestIdFactory : IRequestIdFactory
    {
        private readonly string _id;

        public RequestIdFactory()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string Id()
        {
            return _id;
        }
    }
}
