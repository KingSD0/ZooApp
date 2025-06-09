using Microsoft.AspNetCore.Mvc;
using ZooApp.Services;

namespace ZooApp.Controllers.Api
{
    /// <summary>
    /// API-controller voor dierentuinacties op het niveau van de hele zoo.
    /// Alle acties zijn ook beschikbaar via de webinterface (MVC), en worden gedeeld via de ZooService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ZooApiController : ControllerBase
    {
        private readonly IZooService _zooService;

        /// <summary>
        /// Constructor met dependency injection van de ZooService.
        /// </summary>
        public ZooApiController(IZooService zooService)
        {
            _zooService = zooService;
        }

        /// <summary>
        /// Haalt per verblijf op welke dieren wakker zijn bij zonsopkomst.
        /// </summary>
        /// <returns>Een lijst met verblijfnaam en status per dier.</returns>
        [HttpGet("sunrise")]
        public async Task<IActionResult> Sunrise()
        {
            var result = await _zooService.GetSunriseOverviewAsync();
            return Ok(result); // 200 OK met sunrise-status per verblijf
        }

        /// <summary>
        /// Haalt per verblijf op welke dieren wakker zijn bij zonsondergang.
        /// </summary>
        /// <returns>Een lijst met verblijfnaam en status per dier.</returns>
        [HttpGet("sunset")]
        public async Task<IActionResult> Sunset()
        {
            var result = await _zooService.GetSunsetOverviewAsync();
            return Ok(result);
        }

        /// <summary>
        /// Geeft een overzicht van wat dieren eten per verblijf, inclusief waarschuwingen voor roofdier/prooidiercombinaties.
        /// </summary>
        /// <returns>Een lijst met dieetdetails en waarschuwingen.</returns>
        [HttpGet("feedingtime")]
        public async Task<IActionResult> FeedingTime()
        {
            var result = await _zooService.GetFeedingOverviewAsync();
            return Ok(result);
        }

        /// <summary>
        /// Controleert of verblijven voldoen aan ruimte- en veiligheidseisen.
        /// </summary>
        /// <returns>Een lijst met constraintstatus per verblijf.</returns>
        [HttpGet("checkconstraints")]
        public async Task<IActionResult> CheckConstraints()
        {
            var result = await _zooService.GetConstraintOverviewAsync();
            return Ok(result);
        }

        /// <summary>
        /// Wijs dieren automatisch toe aan verblijven, op basis van strategie.
        /// Strategie 'nieuw' verwijdert bestaande indeling, 'aanvullen' vult bestaande verblijven aan.
        /// </summary>
        /// <param name="strategy">Strategie: 'nieuw' of 'aanvullen'</param>
        /// <returns>200 OK met bevestigingsbericht, of 400 BadRequest bij ongeldige strategie.</returns>
        [HttpPost("autoassign")]
        public async Task<IActionResult> AutoAssign([FromQuery] string strategy)
        {
            if (strategy != "nieuw" && strategy != "aanvullen")
                return BadRequest("Strategie moet 'nieuw' of 'aanvullen' zijn.");

            await _zooService.AutoAssignAsync(strategy);
            return Ok($"AutoAssign uitgevoerd met strategie '{strategy}'.");
        }
    }
}
