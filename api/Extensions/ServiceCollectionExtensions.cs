using Waffle.Core.Foundations;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Senders;
using Waffle.Core.Services;
using Waffle.Core.Services.Branches;
using Waffle.Core.Services.Contacts;
using Waffle.Core.Services.Departments;
using Waffle.Core.Services.Districts;
using Waffle.Core.Services.Ecommerces;
using Waffle.Core.Services.Events;
using Waffle.Core.Services.KeyIn;
using Waffle.Core.Services.Provinces;
using Waffle.Core.Services.Teams;
using Waffle.Data.ContentGenerators;
using Waffle.Infrastructure.Repositories;
using Waffle.Services;

namespace Waffle.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<IFileService, FileExplorerService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<ILocalizationRepository, LocalizationRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkService, WorkService>();
        services.AddScoped<IWorkContentRepository, WorkItemRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderSerivce>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<IMigrationService, MigrationService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IKeyInService, KeyInService>();
        services.AddScoped<ILoanService, LoanService>();

        services.AddScoped<IProvinceRepository, ProvinceRepository>();
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IDistrictService, DistrictService>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IBranchService, BranchService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IGenerator, LeafGenerator>();
        services.AddScoped<IGenerator, ComponentGenerator>();

        services.AddScoped<ITourResortService, TourResortService>();
        services.AddTransient<ITelegramService, TelegramService>();
        services.AddScoped<INotificationService, NotificationService>();
    }
}
