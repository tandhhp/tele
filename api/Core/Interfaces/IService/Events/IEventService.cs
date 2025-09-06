using Waffle.Core.Services.Events.Models;
using Waffle.Entities;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService.Events;

public interface IEventService
{
    Task<TResult> AddSaleRevenueAsync(AddSaleRevenue args);
    Task<TResult> BackToCheckinAsync(BackToCheckin args);
    Task<object?> ListSaleRevenueAsync(SaleRevenueFilterOptions filterOptions);
    Task<TResult> UpdateSaleRevenueAsync(AddSaleRevenue args);
    Task<TResult> DeleteSaleRevenueAsync(Lead lead);
    Task<ListResult<object>> ListKeyInRevenueAsync(SaleRevenueFilterOptions filterOptions);
    Task<object?> RevenueHistoriesAsync(KeyInRevenueFilterOptions filterOptions);
    Task<ListResult<object>> GetListAsync(EventFilterOptions filterOptions);
    Task<TResult> DeleteAsync(int id);
    Task<TResult> UpdateAsync(EventUpdateArgs args);
    Task<TResult> CreateAsync(EventCreateArgs args);
    Task<TResult<object>> DetailAsync(int id);
}
