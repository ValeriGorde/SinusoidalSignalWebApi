using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using SinusoidalSignalWebApi.Models;
using SinusoidalSignalWebApi.Services;

namespace SinusoidalSignalWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SinusoidalSignalController : ControllerBase
    {
        [HttpPost]
        public IActionResult GenerateSignal(SinusoidalSignalParams parametrs)
        {
            try
            {
                MemoryStream imageStream = Calculating.GenerateSignal(parametrs);

                // Отправляем изображение клиенту в виде графического файла
                return File(imageStream, "image/png", "sinusoidal_signal.png");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при генерации изображения {ex.Message}");
            }    
        }
    }
}
