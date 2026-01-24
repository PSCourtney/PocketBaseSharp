using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using PocketBaseSharp;

namespace Example.Pages.Login.Components
{
    public partial class VerifyOtpForm
    {
        [Inject]
        public PocketBase PocketBase { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Parameter, EditorRequired]
        public string OtpId { get; set; } = null!;

        [Parameter, EditorRequired]
        public string Email { get; set; } = null!;

        public string? OtpCode { get; set; }
        private bool _isLoading = false;

        protected async Task VerifyOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                Snackbar.Add("OTP code is required.", Severity.Warning);
                return;
            }

            _isLoading = true;
            try
            {
                var result = await PocketBase.User.AuthWithOtpAsync(OtpId, OtpCode);
                if (result.IsSuccess)
                {
                    Snackbar.Add("Logged in successfully!", Severity.Success);
                    var claims = PocketBaseAuthenticationStateProvider.ParseClaimsFromJwt(result.Value.Token);
                    ((PocketBaseAuthenticationStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(claims);
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    Snackbar.Add($"Invalid OTP code: {string.Join(", ", result.Errors.Select(e => e.Message))}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error verifying OTP: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
