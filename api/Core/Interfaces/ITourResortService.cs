using Waffle.Entities.TourResort;
using Waffle.Models.TourResort;

namespace Waffle.Core.Interfaces
{
    public interface ITourResortService
    {
        Task<List<ListModel>> GetTourResortsListAsync();
        Task<DetailModel> GetTourResortDetailAsync(Guid id);
    }
}
