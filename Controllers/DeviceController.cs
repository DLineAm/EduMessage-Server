using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalIRServerTest.Models;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class DeviceController : Controller
    {
        private UnitOfWork _unitOfWork;

        public DeviceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                return false;
            }
        }
    }
}
