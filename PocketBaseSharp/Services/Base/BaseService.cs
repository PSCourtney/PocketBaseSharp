using PocketBaseSharp.Models;
using System.Web;

namespace PocketBaseSharp.Services.Base
{
    /// <summary>
    /// Abstract base class for all PocketBase service classes.
    /// Provides common functionality for URL encoding, body construction, and property management.
    /// </summary>
    public abstract class BaseService
    {
        private readonly string[] _itemProperties;

        /// <summary>
        /// Initializes a new instance of the BaseService class.
        /// </summary>
        protected BaseService()
        {
            this._itemProperties = this.GetPropertyNames().ToArray();
        }

        /// <summary>
        /// Gets the base API path for this service. Must be implemented by derived classes.
        /// </summary>
        /// <param name="path">Optional additional path segment</param>
        /// <returns>The base API path for this service</returns>
        protected abstract string BasePath(string? path = null);

        /// <summary>
        /// Constructs a request body dictionary from an object, excluding base model properties.
        /// </summary>
        /// <param name="item">The object to convert to a request body</param>
        /// <returns>A dictionary containing the object's properties as key-value pairs</returns>
        protected Dictionary<string, object> ConstructBody(object item)
        {
            var body = new Dictionary<string, object>();

            foreach (var prop in item.GetType().GetProperties())
            {
                if (_itemProperties.Contains(prop.Name)) continue;
                var propValue = prop.GetValue(item, null);
                if (propValue is not null) body.Add(ToCamelCase(prop.Name), propValue);
            }

            return body;
        }

        private string ToCamelCase(string str)
        {
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        private IEnumerable<string> GetPropertyNames()
        {
            return from prop in typeof(BaseModel).GetProperties()
                   select prop.Name;
        }

        /// <summary>
        /// URL-encodes a parameter value for safe inclusion in URLs.
        /// </summary>
        /// <param name="param">The parameter value to encode</param>
        /// <returns>The URL-encoded parameter value, or an empty string if the parameter is null</returns>
        protected string UrlEncode(string? param)
        {
            return HttpUtility.UrlEncode(param) ?? "";
        }

    }
}
