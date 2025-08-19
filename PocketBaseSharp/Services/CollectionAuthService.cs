using PocketBaseSharp.Models;
using PocketBaseSharp.Models.Auth;
using PocketBaseSharp.Models.Log;
using System.Web;
using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing authentication operations on collections that store auth records.
    /// Provides a simplified interface for the most common auth record type.
    /// </summary>
    /// <typeparam name="T">The type of the auth model, must implement IBaseAuthModel</typeparam>
    public class CollectionAuthService<T> : CollectionAuthService<RecordAuthModel<T>, T>
        where T : IBaseAuthModel
    {
        /// <summary>
        /// Initializes a new instance of the CollectionAuthService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        /// <param name="collectionName">The name of the authentication collection</param>
        public CollectionAuthService(PocketBase client, string collectionName) : base(client, collectionName) { }
    }

    /// <summary>
    /// Service for managing authentication operations on collections that store auth records.
    /// Provides full authentication functionality including login, registration, and user management.
    /// </summary>
    /// <typeparam name="R">The record type that contains the auth model</typeparam>
    /// <typeparam name="T">The type of the auth model, must implement IBaseAuthModel</typeparam>
    public class CollectionAuthService<R, T> : BaseAuthService<R>
        where R : RecordAuthModel<T>
        where T : IBaseAuthModel
    {
        protected override string BasePath(string? url = null) => $"/api/collections/{this._collectionName}";

        private readonly PocketBase _client;
        private readonly string _collectionName;
        
        /// <summary>
        /// Initializes a new instance of the CollectionAuthService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        /// <param name="collectionName">The name of the authentication collection</param>
        public CollectionAuthService(PocketBase client, string collectionName) : base(client)
        {
            this._client = client;
            this._collectionName = collectionName;
        }

        public async Task<Result<R>> AuthenticateViaOAuth2Async(string provider, string code, string codeVerifier, string redirectUrl, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("provider", provider);
            body.Add("code", code);
            body.Add("codeVerifier", codeVerifier);
            body.Add("redirectUrl", redirectUrl);

            var url = $"{BasePath()}/auth-via-oauth2";
            var result = await _client.SendAsync<R>(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Result<R> AuthenticateViaOAuth2(string provider, string code, string codeVerifier, string redirectUrl, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("provider", provider);
            body.Add("code", code);
            body.Add("codeVerifier", codeVerifier);
            body.Add("redirectUrl", redirectUrl);

            var url = $"{BasePath()}/auth-via-oauth2";
            var result = _client.Send<R>(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Task<Result> RequestVerificationAsync(string email, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("email", email);

            var url = $"{BasePath()}/request-verification";
            return _client.SendAsync(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public Result RequestVerification(string email, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("email", email);

            var url = $"{BasePath()}/request-verification";
            return _client.Send(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public async Task<Result<R>> ConfirmVerificationAsync(string token, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("token", token);

            var url = $"{BasePath()}/confirm-verification";
            var result = await _client.SendAsync<R>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Result<R> ConfirmVerification(string token, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("token", token);

            var url = $"{BasePath()}/confirm-verification";
            var result = _client.Send<R>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Task<Result> RequestEmailChangeAsync(string newEmail, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("newEmail", newEmail);

            var url = $"{BasePath()}/request-email-change";
            return _client.SendAsync(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public Result RequestEmailChange(string newEmail, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("newEmail", newEmail);

            var url = $"{BasePath()}/request-email-change";
            return _client.Send(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public async Task<Result<R>> ConfirmEmailChangeAsync(string emailChangeToken, string userPassword, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("token", emailChangeToken);
            body.Add("password", userPassword);

            var url = $"{BasePath()}/confirm-email-change";
            var result = await _client.SendAsync<R>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Result<R> ConfirmEmailChange(string emailChangeToken, string userPassword, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("token", emailChangeToken);
            body.Add("password", userPassword);

            var url = $"{BasePath()}/confirm-email-change";
            var result = _client.Send<R>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);

            return SetAndReturn(result);
        }

        public Task<Result<AuthMethodsList>> GetAuthenticationMethodsAsync(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/auth-methods";
            return _client.SendAsync<AuthMethodsList>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        public Result<AuthMethodsList> GetAuthenticationMethods(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/auth-methods";
            return _client.Send<AuthMethodsList>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        public Task<Result<IEnumerable<ExternalAuthModel>>> GetExternalAuthenticationMethodsAsync(string userId, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/records/{HttpUtility.HtmlEncode(userId)}/external-auths";
            return _client.SendAsync<IEnumerable<ExternalAuthModel>>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        public Result<IEnumerable<ExternalAuthModel>> GetExternalAuthenticationMethods(string userId, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/records/{HttpUtility.HtmlEncode(userId)}/external-auths";
            return _client.Send<IEnumerable<ExternalAuthModel>>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        public Task<Result> UnlinkExternalAuthenticationAsync(string userId, string provider, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/records/{HttpUtility.HtmlEncode(userId)}/external-auths/{HttpUtility.HtmlEncode(provider)}";
            return _client.SendAsync(url, HttpMethod.Delete, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public Result UnlinkExternalAuthentication(string userId, string provider, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/records/{HttpUtility.HtmlEncode(userId)}/external-auths/{HttpUtility.HtmlEncode(provider)}";
            return _client.Send(url, HttpMethod.Delete, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

    }
}
