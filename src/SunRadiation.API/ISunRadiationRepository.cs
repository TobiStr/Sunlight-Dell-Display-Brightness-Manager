using System.Threading.Tasks;

namespace SunRadiation.API
{
    public interface ISunRadiationRepository
    {
        Task<int> GetCurrentRadiation();
    }
}