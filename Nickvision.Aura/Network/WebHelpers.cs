using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nickvision.Aura.Network;

/// <summary>
/// A utility class for working with the web
/// </summary>
public static class WebHelpers
{
    /// <summary>
    /// A static HttpClient
    /// </summary>
    /// <remarks>Microsoft recommends not creating and using HttpClient objects over and over again</remarks>
    public static HttpClient Client { get; }

    /// <summary>
    /// Constructs WebHelpers statically
    /// </summary>
    static WebHelpers()
    {
        Client = new HttpClient();
    }

    /// <summary>
    /// Gets whether or not a Uri leads to a valid website
    /// </summary>
    /// <param name="uri">Uri</param>
    /// <returns>True if connection successful, else false</returns>
    public static async Task<bool> GetIsValidWebsiteAsync(this Uri uri) => (await Client.GetAsync(uri)).StatusCode != HttpStatusCode.NotFound;
}
