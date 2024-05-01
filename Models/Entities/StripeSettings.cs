
namespace Models.Entities;

public class StripeSettings
{
    public string PublicKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
}
