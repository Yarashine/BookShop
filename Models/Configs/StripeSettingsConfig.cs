
namespace Models.Configs;


public class StripeSessionConfig
{
    public string SuccessUrl { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string SessionMode { get; set; } = null!;
}
