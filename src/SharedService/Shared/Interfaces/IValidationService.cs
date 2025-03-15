namespace Shared.Interfaces
{
    public interface IValidationService
    {
        Task ValidateEntityExistsAsync<T>(Func<int, Task<T>> getByIdFunc, int id, string entityName);
    }
}
