
namespace BirthCenter.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message, 400)
        {
        }
    }
}