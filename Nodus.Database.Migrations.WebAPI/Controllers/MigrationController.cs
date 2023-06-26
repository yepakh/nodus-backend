using Microsoft.AspNetCore.Mvc;
using Nodus.Database.Migrator;

namespace Nodus.Database.Migrations.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly MigrationService _migrationService;
        private readonly Config _config;

        public MigrationController(MigrationService migrationService)
        {
            _migrationService = migrationService;
            _config = new Config();
        }

        [HttpGet("MigrateAdmin")]
        public async Task<IActionResult> MigrateAdmin()
        {
            try
            {
                await _migrationService.MigrateAdminDb(_config);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("MigrateClients")]
        public async Task<IActionResult> MigrateClients()
        {
            try
            {
                await _migrationService.MigrateClientDbs(_config);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("MigrateOneClient")]
        public async Task<IActionResult> MigrateOneClient([FromRoute] int companyId)
        {
            try
            {
                await _migrationService.MigrateOneClientDb(_config, companyId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("MigrateAll")]
        public async Task<IActionResult> MigrateAll()
        {
            try
            {
                await _migrationService.MigrateBothDbs(_config);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}