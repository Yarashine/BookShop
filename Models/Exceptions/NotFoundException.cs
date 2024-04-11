
namespace Models.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entity) : base($"{entity} not found.") { }
}
