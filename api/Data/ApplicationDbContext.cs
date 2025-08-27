using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Waffle.Entities;
using Waffle.Entities.Contacts;
using Waffle.Entities.Ecommerces;
using Waffle.Entities.Healthcares;
using Waffle.Entities.Payments;
using Waffle.Entities.Plasma;
using Waffle.Entities.TourResort;
using Waffle.Entities.Tours;
using Waffle.Entities.Users;

namespace Waffle.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public virtual DbSet<AppLog> AppLogs { get; set; }
    public virtual DbSet<AppSetting> AppSettings { get; set; }
    public virtual DbSet<Catalog> Catalogs { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Component> Components { get; set; }
    public virtual DbSet<FileContent> FileContents { get; set; }
    public virtual DbSet<WorkContent> WorkContents { get; set; }
    public virtual DbSet<WorkItem> WorkItems { get; set; }
    public virtual DbSet<Contact> Contacts { get; set; }
    public virtual DbSet<Localization> Localizations { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<TourResort> TourResorts { get; set; }
    public virtual DbSet<TourResortComment> TourResortComments { get; set; }
    public virtual DbSet<TourResortImage> TourResortImages { get; set; }
    public virtual DbSet<TourResortItinerary> TourResortItineraries { get; set; }
    public virtual DbSet<TourResortOrder> TourResortOrders { get; set; }
    public virtual DbSet<TourResortSurvey> TourResortSurveys { get; set; }
    public virtual DbSet<TourCatalog> TourCatalogs { get; set; }
    public virtual DbSet<Itinerary> Itineraries { get; set; }
    public virtual DbSet<Form> Forms { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<Healthcare> Healthcares { get; set; }
    public virtual DbSet<Card> Cards { get; set; }
    public virtual DbSet<SubUser> SubUsers { get; set; }
    public virtual DbSet<ContactActivity> ContactActivities { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<TourAmenity> TourAmenities { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Loyalty> Loyalties { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<UserTopup> UserTopups { get; set; }
    public DbSet<UserChange> UserChanges { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<LeadFeedback> LeadFeedbacks { get; set; }
    public DbSet<CardHolderQueue> CardHolderQueues { get; set; }
    public DbSet<SubLead> SubLeads { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<TableStatus> TableStatuses { get; set; }
    public DbSet<LeadHistory> LeadHistories { get; set; }
    public DbSet<LeadProcess> LeadProcesses { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<TopupEvidence> TopupEvidences { get; set; }
    public DbSet<UserPoint> UserPoints { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<JobKind> JobKinds { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Department> Departments { get; set; }

    #region PlasmaUser
    public DbSet<PlasmaUser> PlasmaUsers { get; set; }
    public DbSet<PlasmaCheckIn> PlasmaCheckIns { get; set; }
    #endregion
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Transport> Transports { get; set; }
    public DbSet<CallStatus> CallStatuses { get; set; }
    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<WorkItem>().HasKey(k => new { k.WorkId, k.CatalogId });
        builder.Entity<TourAmenity>().HasKey(k => new { k.CatalogId, k.AmenityId });
    }
}