using balcheckcalcweb.Models;
using System.Threading.Tasks;

namespace balcheckcalcweb.Services
{
    public interface IPdfService
    {
        byte[] GenerateCalculationPdf(CheckHistory checkHistory);
    }
}