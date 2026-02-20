using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalDecoder.Domain.Interfaces;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Api.Controllers
{
    [Route("api/signal")]
    [ApiController]
    public class SignalController(ISignalSimulatorService SignalSimulatorService) : ControllerBase
    {
        private readonly ISignalSimulatorService _SignalSimulatorService = SignalSimulatorService;

        [HttpPost]
        [Route("simulate")]
        public IActionResult SimulateDevices([FromBody]SimulateRequest _Request)
        {
            int SignalLength = _Request.Devices.First().Value.Length;

            foreach (KeyValuePair<string, int[]> Device in _Request.Devices)
            {
                if (SignalLength != Device.Value.Length)
                {
                    return BadRequest("All signal patterns must have the same length.");
                }

                if (!Device.Value.All(v => v >= 0))
                {
                    return BadRequest("All signal patterns must contain non negative numbers.");
                }
            }

            return Ok(_SignalSimulatorService.Simulate(_Request.Devices));
        }
    }
}
