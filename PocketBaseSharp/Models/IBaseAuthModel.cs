namespace PocketBaseSharp.Models
{
    public interface IBaseAuthModel : IBaseModel
    {
        string? Email { get; }

        bool? EmailVisibility { get; }

        string? Username { get; }

        bool? Verified { get; }
    }
}
