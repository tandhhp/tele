using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces;
using Waffle.Data;

namespace Waffle.Controllers
{
    [Route("api/resort")]
    public class TourResortController : BaseController
    {
        private readonly ITourResortService _tourResortService;
        private readonly ApplicationDbContext _context;

        public TourResortController(ITourResortService tourResortService, ApplicationDbContext context)
        {
            _tourResortService = tourResortService;
            _context = context;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetTourResortsListAsync()
        {
            var result = await _tourResortService.GetTourResortsListAsync();
            //foreach (var item in result)
            //{
            //    var catalog = new Entities.Catalog
            //    {
            //        Active = true,
            //        CreatedDate = DateTime.Now,
            //        Name = item.Name,
            //        Thumbnail = item.Thumbnail,
            //        ViewCount = 0,
            //        Type = Entities.CatalogType.Tour,
            //        ModifiedDate = DateTime.Now,
            //        NormalizedName = SeoHelper.ToSeoFriendly(item.Name)
            //    }; 
            //    await _context.Catalogs.AddAsync(catalog);

            //    await _context.SaveChangesAsync();

            //    var images = await _context.TourResortImages.Where(x => x.TourResortId == item.Id).Select(x => x.Url).ToListAsync();

            //    await _context.TourCatalogs.AddAsync(new Entities.Tours.TourCatalog
            //    {
            //        Amenities = "Wifi,Tủ lạnh,Điều hòa",
            //        CatalogId = catalog.Id,
            //        Images = string.Join(",", images),
            //        Location = item.Address,
            //        Point = item.Point,
            //        Tags = "Buffet,Ăn sáng,Nhà hàng,Biển"
            //    });
            //}
            //await _context.SaveChangesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourResortDetailAsync([FromRoute] Guid id) 
            => Ok(await _tourResortService.GetTourResortDetailAsync(id));
    }
}
