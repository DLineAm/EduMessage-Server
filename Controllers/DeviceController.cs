using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalIRServerTest.Models;
using SignalIRServerTest.Services;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class DeviceController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(UnitOfWork unitOfWork, ILogger<DeviceController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("Validate")]
        public async Task<bool> ValidateDevice([FromBody] byte[] serialNumber)
        {
            var foundDevice = _unitOfWork.DeviceRepository.GetById(serialNumber);
            return foundDevice == null;
        }

        [HttpPost]
        public async Task<bool> AddDevice([FromBody] Device device)
        {
            try
            {
                if (device.IdUserNavigation != null)
                {
                    device.IdUserNavigation = null;
                }

                _unitOfWork.DeviceRepository.Insert(device);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(AddDevice)));
                return false;
            }
        }
    }
}
