using MailKit.Net.Smtp;
using MimeKit;
using Waffle.Entities;

namespace Waffle.ExternalAPI;

public class Sender
{
    public static async Task SendAsync(string? recipientEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(recipientEmail))
        {
            return;
        }
        string senderEmail = "noreply@nuras.com.vn";
        string password = "3M@ail0f764h37hc";

        // Mail message
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress("No Reply", senderEmail));
        mail.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
        mail.Subject = subject;
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        mail.Body = bodyBuilder.ToMessageBody();
        // SMTP client
        using var client = new SmtpClient();
        await client.ConnectAsync("mail.nuras.com.vn", 465, true);
        await client.AuthenticateAsync(senderEmail, password);

        try
        {
            // Send the email
            await client.SendAsync(mail);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email: " + ex.Message);
        }
    }

    public static async Task SendTransAsync(ApplicationUser user, Catalog catalog, int point)
    {
        var service = "Dưỡng sinh độc bản";
        if (catalog.Type == CatalogType.Healthcare)
        {
            service = "Đặt lịch khám";
        }
        if (catalog.Type == CatalogType.Product)
        {
            service = "Chăm sóc sức khỏe";
        }
        // Ngày hiện tại
        DateTime currentDate = DateTime.Now;

        // Tính số năm
        int years = currentDate.Year - user.ContractDate.GetValueOrDefault().Year;

        // Kiểm tra nếu ngày ký hợp đồng đã qua trong năm hiện tại hay chưa
        if (currentDate < user.ContractDate.GetValueOrDefault().AddYears(years))
        {
            years--;
        }
        var subject = "[NURA'S] Xác nhận sử dụng dịch vụ và trừ điểm";
        var body = $@"
<i>Kính chào <b>Quý Chủ Thẻ {user.Name}</b></i> <br/>
<i><b>Thân mến,</b></i> <br/>
<b>Phòng Trải nghiệm Khách hàng NURA'S</b> xin gửi <b>“XÁC NHẬN SỬ DỤNG DỊCH VỤ VÀ TRỪ ĐIỂM”</b><br/>
Cám ơn <b>Quý Chủ Thẻ {user.Name}</b> đã quyết định lựa chọn <b>NURA'S</b> để đồng hành cùng gia đình. Chúng tôi xin xác nhận dịch vụ như sau:<br/>
<ul>
<li>Dịch vụ sử dụng điểm: <b>{service}</b></li>
<li>Ngày đăng ký sử dụng: <b>{DateTime.Now:dd/MM/yyyy}</b></li>
<li>Họ và tên người sử dụng: <b>{user.Name}</b></li>
</ul>
<div style='color:tomato'><b>***[{user.Name}] sử dụng {point}NP, còn lại {user.Loyalty}NP cho năm thứ {years + 1}</div>
<div><b style='color:tomato'>Lưu ý:</b> Những chi phí phát sinh ngoài Dịch vụ đã đăng ký, Quý khách vui lòng tự túc thanh toán và sẽ không quy đổi thành điểm NP.</div>
Chúc <b>Quý Chủ Thẻ và Gia Đình</b> có những trải nghiệm đẳng cấp cùng <b>NURA'S.</b><br/>
Nếu cần hỗ trợ hoặc cần thêm thông tin vui lòng liên hệ:<br/>
<b>Hotline</b>: 090.901.3386 | 099.661.5028<br/>
<b>Thứ 2 - Chủ nhật (7h30-17h)</b><br/>
<b>Hoặc Trợ lý cá nhân</b><br/>
Trân trọng
";
        await SendAsync(user.Email, subject, body);
    }

    public static async Task SendCompleteAsync(ApplicationUser user, Catalog catalog, int point)
    {
        var service = "Chúc <b>Quý Chủ Thẻ và Gia đình</b> có chuyến đi ý nghĩa và tràn đầy niềm vui cùng <b>NURA'S.</b><br/>";
        if (catalog.Type != CatalogType.Tour)
        {
            service = "Chúc <b>Quý Chủ Thẻ và Gia đình</b> có một trải nghiệm sức khỏe trọn vẹn cùng <b>NURA'S.</b><br/>";
        }
        var subject = "[NURA'S] Xác nhận sử dụng dịch vụ và trừ điểm";
        var body = $@"
<i>Kính chào <b>Quý Chủ Thẻ {user.Name}</b></i> <br/>
<i><b>Thân mến,</b></i> <br/>
Cảm ơn <b>Quý Chủ Thẻ {user.Name}</b> đã quyết định lựa chọn <b>NURA'S</b> để đồng hành cùng sức khỏe và gia đình<br/>
<b>Phòng Trải nghiệm Khách hàng NURA'S</b> xin gửi <b>“XÁC NHẬN ĐÃ SỬ DỤNG ĐIỂM NURA'S”</b> như sau:<br/>
<ul>
<li><b>Họ và tên</b>: <b>{user.Name}</b></li>
<li><b>Mã hợp đồng</b>: <b>{DateTime.Now:dd/MM/yyyy}</b></li>
<li><b>Điểm NP đã sử dụng</b>: <b>{point}</b></li>
</ul>
{service}
Nếu cần hỗ trợ hoặc cần thêm thông tin vui lòng liên hệ:<br/>
<b>Hotline</b>: 090.901.3386 | 099.661.5028<br/>
<b>Thứ 2 - Chủ nhật (7h30-17h)</b><br/>
<b>Hoặc Trợ lý cá nhân</b><br/>
Trân trọng
";
        await SendAsync(user.Email, subject, body);
    }
}
