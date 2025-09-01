using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IRepository.Events;
using Waffle.Core.Interfaces.IService.Events;
using Waffle.Core.Services.Tables.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Services.Tables;

public class TableService(ITableRepository _tableRepository, ICurrentUser _currentUser, IRoomService _roomService) : ITableService
{
    public async Task<TResult> CreateAsync(TableCreateArgs args)
    {
        var room = await _roomService.FindAsync(args.RoomId);
        if (room is null) return TResult.Failed("Room not found");
        await _tableRepository.AddAsync(new Table
        {
            Status = args.Status,
            Name = args.Name,
            CreatedBy = _currentUser.GetId(),
            CreatedDate = DateTime.Now,
            RoomId = args.RoomId,
            SortOrder = args.SortOrder
        });
        return TResult.Success;
    }

    public async Task<TResult> DeleteAsync(int id)
    {
        var data = await _tableRepository.FindAsync(id);
        if (data is null) return TResult.Failed("Table not found");
        await _tableRepository.DeleteAsync(data);
        return TResult.Success;
    }

    public Task<ListResult<object>> ListAsync(TableFilterOptions filterOptions) => _tableRepository.ListAsync(filterOptions);

    public async Task<TResult> UpdateAsync(TableUpdateArgs args)
    {
        var data = await _tableRepository.FindAsync(args.Id);
        if (data is null) return TResult.Failed("Table not found");
        var room = await _roomService.FindAsync(args.RoomId);
        if (room is null) return TResult.Failed("Room not found");
        data.Name = args.Name;
        data.ModifiedBy = _currentUser.GetId();
        data.ModifiedDate = DateTime.Now;
        data.Status = args.Status;
        data.SortOrder = args.SortOrder;
        data.RoomId = args.RoomId;
        await _tableRepository.UpdateAsync(data);
        return TResult.Success;
    }
}
