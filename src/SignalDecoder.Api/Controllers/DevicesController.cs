using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalDecoder.Domain.Interfaces;

namespace SignalDecoder.Api.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DevicesController(IDeviceGeneratorService DeviceGeneratorService) : ControllerBase
    {
        private readonly IDeviceGeneratorService _DeviceGeneratorService = DeviceGeneratorService;

        /// <summary>
        /// Generates a random set of devices with signal patterns
        /// </summary>
        [HttpGet]
        [Route("generate")]
        public IActionResult DevicesGenerator([FromQuery]int count = 5, [FromQuery]int signalLength = 4, [FromQuery]int maxStrength = 9)
        {
            if (count < 1  || count > 100)
            {
                return BadRequest("Invalid Parameter Range, Count must be between 1 and 100.");
            }

            if (signalLength <  1 || signalLength > 20)
            {
                return BadRequest("Invalid Parameter Range, Count must be between 1 and 20.");
            }

            if (maxStrength < 1 || maxStrength > 100)
            {
                return BadRequest("Invalid Parameter Range, Count must be between 1 and 100.");
            }

            return Ok(_DeviceGeneratorService.GenerateDevices(count, signalLength, maxStrength));
        }
    }
}
