using Grpc.Core;
using Nodus.Database.Migrations.gRPC;
using Nodus.Database.Migrator;

namespace Nodus.Database.Migrations.gRPC.Services
{
    public class MigrationGrpcService : Migrator.MigratorBase
    {
        private readonly MigrationService _migrationService;
        private readonly Config _config;
        public MigrationGrpcService(MigrationService migrationService)
        {
            _migrationService = migrationService;
            _config = new Config();
        }

        public override async Task<SuccessReply> MigrateAdmin(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                await _migrationService.MigrateAdminDb(_config);

                return new SuccessReply();
            }
            catch (Exception ex)
            {
                return new SuccessReply() { ErrorMessage = ex.Message };
            }
        }

        public override async Task<SuccessReply> MigrateAllClients(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                await _migrationService.MigrateClientDbs(_config);

                return new SuccessReply();
            }
            catch (Exception ex)
            {
                return new SuccessReply() { ErrorMessage = ex.Message };
            }
        }

        public override async Task<SuccessReply> MigrateOneClient(MigrateCompanyRequest request, ServerCallContext context)
        {
            try
            {
                await _migrationService.MigrateOneClientDb(_config, request.CompanyId);

                return new SuccessReply();
            }
            catch (Exception ex)
            {
                return new SuccessReply() { ErrorMessage = ex.Message };
            }
        }

        public override async Task<SuccessReply> MigrateAll(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                await _migrationService.MigrateBothDbs(_config);

                return new SuccessReply();
            }
            catch (Exception ex)
            {
                return new SuccessReply() { ErrorMessage = ex.Message };
            }
        }
    }
}