using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Interfaces;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Entities.TourResort;
using Waffle.Models.TourResort;

namespace Waffle.Core.Services
{
    public class TourResortService : ITourResortService
    {
        private readonly ApplicationDbContext _context;

        public TourResortService(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListModel>> GetTourResortsListAsync()
        {
            var tours = await _context.TourResorts.ToListAsync();
            var images = await _context.TourResortImages.ToListAsync();
            var res = new List<ListModel>();
            foreach(var tour in tours)
            {
                var tourImage = images.FirstOrDefault(x => x.TourResortId == tour.Id);
                res.Add(new ListModel { 
                    Id = tour.Id,
                    Address = tour.Address,
                    Duration = tour.Duration,
                    Exclude = tour.Exclude,
                    Thumbnail = tourImage == null ? "" : tourImage.Url,
                    Name = tour.Name,
                    Include = tour.Include,
                    Point = tour.Point,
                    Rating = tour.Rating,
                    Summary = tour.Summary
                });
            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DetailModel> GetTourResortDetailAsync(Guid id)
        {
            var tourResort = _context.TourResorts.FirstOrDefault(x => x.Id == id);
            if (tourResort == null)
                return new DetailModel();

            var itineraries = await _context.TourResortItineraries
                .Where(x => x.TourResortId == id)
                .OrderBy(x => x.Title)
                .ToListAsync();
            var comments = await _context.TourResortComments
                .Where(x => x.TourResortId == id)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            var tourImages = await _context.TourResortImages
                .Where(x => x.TourResortId == id)
                .Select(x => x.Url)
                .ToListAsync();

            return new DetailModel
            {
                Id = tourResort.Id,
                Name = tourResort.Name,
                ImageUrls = tourImages,
                Comments = comments,
                Exclude = tourResort.Exclude,
                Include = tourResort.Include,
                Itineraries = itineraries,
                Rating = tourResort.Rating,
                Summary = tourResort.Summary
            };
        }
    }
}
