using Microsoft.AspNetCore.Components;
using MudBlazor;
using PocketBaseSharp;

namespace Example.Pages.Login.Components
{
    public partial class RequestOtpForm
    {
        [Inject]
        public PocketBase PocketBase { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Parameter]
        public Action<string, string>? OnOtpRequested { get; set; }

        public string? Email { get; set; }
        private bool _isLoading = false;

        protected async Task RequestOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Snackbar.Add("Email is required.", Severity.Warning);
                return;
            }

            _isLoading = true;
            try
            {
                var result = await PocketBase.User.RequestOtpAsync(Email);
                if (result.IsSuccess && result.Value.OtpId is not null)
                {
                    Snackbar.Add($"OTP sent to {Email}. Check your email!", Severity.Success);
                    OnOtpRequested?.Invoke(result.Value.OtpId, Email);
                }
                else
                {
                    Snackbar.Add($"Failed to request OTP: {string.Join(", ", result.Errors.Select(e => e.Message))}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error requesting OTP: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}

