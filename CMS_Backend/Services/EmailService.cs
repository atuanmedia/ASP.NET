/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CMS_Backend.Services
{
    public class EmailSettings
    {
        public string SmtpHost    { get; set; } = "smtp.gmail.com";
        public int    SmtpPort    { get; set; } = 587;
        public string SenderEmail { get; set; } = "";
        public string SenderName  { get; set; } = "AT Design Furniture";
        public string Password    { get; set; } = "";
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> opts)
        {
            _settings = opts.Value;
        }

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            string toName,
            int    orderId,
            string orderDate,
            string shippingAddress,
            string? notes,
            List<OrderEmailItem> items)
        {
            var total = items.Sum(i => i.Quantity * i.UnitPrice);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"[AT Design] Xác nhận đơn hàng #{orderId}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildHtml(toName, orderId, orderDate, shippingAddress, notes, items, total)
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        private static string BuildHtml(
            string toName, int orderId, string orderDate,
            string shippingAddress, string? notes,
            List<OrderEmailItem> items, decimal total)
        {
            var rows = string.Join("", items.Select(i =>
                $"""
                <tr>
                  <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0">{i.ProductName}</td>
                  <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:center">{i.Quantity}</td>
                  <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:right">{i.UnitPrice:N0}₫</td>
                  <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:right;font-weight:600">{i.Quantity * i.UnitPrice:N0}₫</td>
                </tr>
                """));

            var notesRow = string.IsNullOrWhiteSpace(notes) ? "" :
                $"""
                <tr>
                  <td colspan="2" style="padding:8px 0;color:#888;font-size:13px">Ghi chú:</td>
                  <td colspan="2" style="padding:8px 0;color:#555;font-size:13px">{notes}</td>
                </tr>
                """;

            return $"""
            <!DOCTYPE html>
            <html lang="vi">
            <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
            <body style="margin:0;padding:0;background:#f5f5f5;font-family:'Segoe UI',Arial,sans-serif">
              <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f5f5;padding:40px 0">
                <tr><td align="center">
                  <table width="600" cellpadding="0" cellspacing="0" style="background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08)">

                    <!-- Header -->
                    <tr>
                      <td style="background:#1a1a1a;padding:36px 40px;text-align:center">
                        <div style="font-size:26px;font-weight:900;color:#fff;letter-spacing:-0.5px">AT Design</div>
                        <div style="font-size:12px;color:rgba(255,255,255,.5);letter-spacing:3px;text-transform:uppercase;margin-top:4px">Furniture & Interior</div>
                      </td>
                    </tr>

                    <!-- Hero -->
                    <tr>
                      <td style="padding:40px 40px 32px;text-align:center">
                        <div style="width:64px;height:64px;background:#f0faf0;border-radius:50%;display:inline-flex;align-items:center;justify-content:center;margin-bottom:20px">
                          <span style="font-size:28px">✅</span>
                        </div>
                        <h1 style="margin:0 0 8px;font-size:24px;font-weight:800;color:#1a1a1a">Đặt hàng thành công!</h1>
                        <p style="margin:0;font-size:15px;color:#888">Cảm ơn <strong style="color:#1a1a1a">{toName}</strong> đã tin tưởng AT Design.</p>
                      </td>
                    </tr>

                    <!-- Order info -->
                    <tr>
                      <td style="padding:0 40px 28px">
                        <table width="100%" cellpadding="0" cellspacing="0" style="background:#f9f9f9;border-radius:12px;padding:20px">
                          <tr>
                            <td style="padding:8px 0;color:#888;font-size:13px;width:50%">Mã đơn hàng:</td>
                            <td style="padding:8px 0;font-weight:700;color:#1a1a1a;font-size:15px">#{orderId}</td>
                          </tr>
                          <tr>
                            <td style="padding:8px 0;color:#888;font-size:13px">Ngày đặt:</td>
                            <td style="padding:8px 0;color:#555;font-size:13px">{orderDate}</td>
                          </tr>
                          <tr>
                            <td style="padding:8px 0;color:#888;font-size:13px">Địa chỉ giao hàng:</td>
                            <td style="padding:8px 0;color:#555;font-size:13px">{shippingAddress}</td>
                          </tr>
                          {notesRow}
                        </table>
                      </td>
                    </tr>

                    <!-- Products table -->
                    <tr>
                      <td style="padding:0 40px 32px">
                        <div style="font-size:14px;font-weight:700;color:#1a1a1a;letter-spacing:1px;text-transform:uppercase;margin-bottom:12px">Chi tiết sản phẩm</div>
                        <table width="100%" cellpadding="0" cellspacing="0" style="border:1px solid #f0f0f0;border-radius:10px;overflow:hidden">
                          <thead>
                            <tr style="background:#f9f9f9">
                              <th style="padding:12px 16px;text-align:left;font-size:12px;color:#888;font-weight:600;text-transform:uppercase;letter-spacing:.5px">Sản phẩm</th>
                              <th style="padding:12px 16px;text-align:center;font-size:12px;color:#888;font-weight:600;text-transform:uppercase;letter-spacing:.5px">SL</th>
                              <th style="padding:12px 16px;text-align:right;font-size:12px;color:#888;font-weight:600;text-transform:uppercase;letter-spacing:.5px">Đơn giá</th>
                              <th style="padding:12px 16px;text-align:right;font-size:12px;color:#888;font-weight:600;text-transform:uppercase;letter-spacing:.5px">Thành tiền</th>
                            </tr>
                          </thead>
                          <tbody>
                            {rows}
                          </tbody>
                        </table>

                        <!-- Total -->
                        <table width="100%" cellpadding="0" cellspacing="0" style="margin-top:16px">
                          <tr>
                            <td style="text-align:right;padding:12px 0">
                              <span style="font-size:14px;color:#888">Tổng cộng: </span>
                              <span style="font-size:22px;font-weight:900;color:#1a1a1a;margin-left:8px">{total:N0}₫</span>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>

                    <!-- Status info -->
                    <tr>
                      <td style="padding:0 40px 40px">
                        <table width="100%" cellpadding="0" cellspacing="0" style="background:#fffbeb;border:1px solid #fde68a;border-radius:10px;padding:16px">
                          <tr>
                            <td>
                              <div style="font-size:13px;color:#92400e;font-weight:600;margin-bottom:6px">🔔 Trạng thái đơn hàng</div>
                              <div style="font-size:13px;color:#78350f;line-height:1.6">
                                Đơn hàng của bạn đang chờ xác nhận từ cửa hàng.<br>
                                Chúng tôi sẽ liên hệ trong vòng <strong>24 giờ</strong> để xác nhận và thông báo thời gian giao hàng.
                              </div>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                      <td style="background:#f9f9f9;padding:24px 40px;text-align:center;border-top:1px solid #f0f0f0">
                        <div style="font-size:13px;color:#aaa;line-height:1.8">
                          Nếu có thắc mắc, vui lòng liên hệ chúng tôi.<br>
                          <strong style="color:#555">AT Design Furniture</strong> — Kiến tạo không gian sống đẳng cấp
                        </div>
                      </td>
                    </tr>

                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
        }
    }
}
