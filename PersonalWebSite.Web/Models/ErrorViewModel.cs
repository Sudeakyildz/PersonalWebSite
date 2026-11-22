namespace PersonalWebSite.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int StatusCode { get; set; } = 500;

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string Title => StatusCode switch
    {
        404 => "Sayfa bulunamadı",
        403 => "Erişim reddedildi",
        401 => "Giriş gerekli",
        _ => "Bir şeyler ters gitti"
    };

    public string Message => StatusCode switch
    {
        404 => "Aradığınız sayfa taşınmış, silinmiş ya da hiç var olmamış olabilir.",
        403 => "Bu sayfayı görüntülemek için yetkiniz yok.",
        401 => "Bu sayfayı görmek için giriş yapmanız gerekiyor.",
        _ => "İsteğiniz işlenirken beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin."
    };
}
