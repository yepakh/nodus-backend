using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace Nodus.Database.Context
{
    public class EFContextFactory
    {
        private readonly string _adminConnectionString;

        private readonly Dictionary<int, String> _dbConnectionStringCache;

        private readonly ReaderWriterLockSlim _lockSlim;

        public EFContextFactory(IOptions<AdminConnectionStringOptions> connectionStringOptions)
        {
            _adminConnectionString = connectionStringOptions.Value.AdminDatabase;
            _dbConnectionStringCache = new Dictionary<int, String>(16);
            _lockSlim = new ReaderWriterLockSlim();
        }

        private async Task<String> GetConnectionStringFromDb(int companyId)
        {
            try
            {
                var result =
                    await new AdminContext(_adminConnectionString).Companies.Where(s => s.Id == companyId).Select(s => s.ConnectionString).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<String> TryGetDivisionConnectionString(int companyId)
        {
            if (companyId < 1)
            {
                return null;
            }

            String result;

            _lockSlim.EnterReadLock();
            try
            {
                if (_dbConnectionStringCache.TryGetValue(companyId, out result))
                {
                    return result;
                }
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }

            result = await GetConnectionStringFromDb(companyId);

            if (String.IsNullOrWhiteSpace(result))
            {
                // Do not store invalid connection strings
                return result;
            }

            _lockSlim.EnterWriteLock();
            try
            {
                _dbConnectionStringCache[companyId] = result;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }

            return result;
        }

        public async Task<ClientContext> GetContext(int companyId)
        {
            var connectionString = await TryGetDivisionConnectionString(companyId);

            return String.IsNullOrWhiteSpace(connectionString) ? null : new ClientContext(connectionString);
        }

        public async Task<ClientContext> GetRequiredContext(int companyId)
        {
            var context = await GetContext(companyId);
            return context ?? throw new Exception($"Context for company with id {companyId} not exists");
        }

        public AdminContext GetAdminContext()
        {
            return new AdminContext(_adminConnectionString);
        }
    }
}
