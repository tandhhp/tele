using Waffle.Entities;

namespace Waffle.Models.ViewModels.Users;

public class CurrentUserViewModel : ApplicationUser
{
    public Tier Tier { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public string? TierColor { get; set; }
    public string? CardBackImage { get; set; }
    public string? CardFrontImage { get; set; }
    public string? TierName { get; set; }
    public string? Seller { get; set; }
    public string? SaleName { get; set; }
    public string? SaleUserName { get; set; }

    public double ExpiredPercentage => ((DateTime.Now - CreatedDate).TotalDays / (LoyaltyExpiredDate.GetValueOrDefault() - CreatedDate).TotalDays) * 100;

    public string? PhoneNumberHide => string.IsNullOrEmpty(PhoneNumber) || PhoneNumber.Length != 10 ? string.Empty : $"******{PhoneNumber.Substring(7, 3)}";

    public string? SmUserName { get; set; }
    public string? SmName { get; set; }

    public bool Expired => LoyaltyExpiredDate < DateTime.Now;

    public int Year { get
        {
            if (ContractDate != null)
            {
                // Ngày hiện tại
                DateTime currentDate = DateTime.Now;

                // Tính số năm
                int years = currentDate.Year - ContractDate.GetValueOrDefault().Year;

                // Kiểm tra nếu ngày ký hợp đồng đã qua trong năm hiện tại hay chưa
                if (currentDate < ContractDate.GetValueOrDefault().AddYears(years))
                {
                    years--;
                }
                return years + 1;
            }
            return 1;
        }
    }

    public bool HasSubContract { get; set; }

    public List<Contract>? SubContracts { get; set; }
}
