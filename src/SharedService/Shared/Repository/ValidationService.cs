using Shared.Interfaces;

namespace Shared.Repository
{
    public class ValidationService : IValidationService
    {
        public async Task ValidateEntityExistsAsync<T>(Func<int, Task<T>> getByIdFunc, int id, string entityName) // very clever idea :)
        {
            if (await getByIdFunc(id) == null)
            {
                throw new KeyNotFoundException($"{entityName} not found.");
            }
        }
    }
}
