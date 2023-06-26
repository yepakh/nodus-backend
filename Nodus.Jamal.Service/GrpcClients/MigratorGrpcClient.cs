using Nodus.Database.Migrations.gRPC;

namespace Nodus.Jamal.Service.GrpcClients
{
    public class MigratorGrpcClient
    {
        private readonly Migrator.MigratorClient _migratorProtoService;

        public MigratorGrpcClient(Migrator.MigratorClient migratorProtoService)
        {
            _migratorProtoService = migratorProtoService;
        }

        public SuccessReply MigrateOneClient(int companyId)
        {
            var request = new MigrateCompanyRequest
            {
                CompanyId = companyId,
            };

            return _migratorProtoService.MigrateOneClient(request);
        }
    }
}
