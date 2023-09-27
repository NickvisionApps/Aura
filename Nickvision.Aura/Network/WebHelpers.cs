using System;
using System.IO;
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
    public static async Task<bool> GetIsValidWebsiteAsync(this Uri uri)
    {
        try
        {
            return (await Client.GetAsync(uri)).StatusCode != HttpStatusCode.NotFound;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Downloads a file from a url to a file on disk
    /// </summary>
    /// <param name="url">The url of the file to download</param>
    /// <param name="path">The path of where to save the file to disk</param>
    /// <returns>True if successful, else false</returns>
    public static async Task<bool> GetFileAsync(this HttpClient client, string url, string path)
    {
        var bytes = await client.GetByteArrayAsync(url);
        if(bytes.Length > 0)
        {
            await File.WriteAllBytesAsync(path, bytes);
        }
        return false;
    }
}
