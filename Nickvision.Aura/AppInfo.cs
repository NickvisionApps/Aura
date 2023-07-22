using Markdig;
using System.Text;

namespace Nickvision.Aura;

/// <summary>
/// A model for the information about the application
/// </summary>
public class AppInfo
{
    /// <summary>
    /// The id of the application
    /// </summary>
    /// <remarks>Should follow <see href="https://www.freedesktop.org/software/appstream/docs/chap-Metadata.html#tag-id-generic">Freedesktop Appstream Specification</see></remarks>
    public string ID { get; set; }
    /// <summary>
    /// The name of the application
    /// </summary>
    /// <remarks>This is original full name, not translated</remarks>
    public string Name { get; set; }
    /// <summary>
    /// The short name of the application
    /// </summary>
    /// <remarks>Can be translated</remarks>
    public string ShortName { get; set; }
    /// <summary>
    /// The description of the application
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// The running version of the application
    /// </summary>
    /// <remarks>Versions containing "-" character are considered development versions</remarks>
    public string Version { get; set; }
    /// <summary>
    /// The changelog in markdown format for the running version of the application
    /// </summary>
    public string Changelog { get; set; }
    /// <summary>
    /// The source repo for the application
    /// </summary>
    public Uri SourceRepo { get; set; }
    /// <summary>
    /// The issue tracker url for the application
    /// </summary>
    public Uri IssueTracker { get; set; }
    /// <summary>
    /// The support url for the application
    /// </summary>
    public Uri SupportUrl { get; set; }
    /// <summary>
    /// The dictionary of extra urls
    /// </summary>
    public Dictionary<string, Uri> ExtraLinks { get; set; }
    /// <summary>
    /// The dictionary of developers' names and urls
    /// </summary>
    public Dictionary<string, Uri> Developers { get; set; }
    /// <summary>
    /// The dictionary of designers' names and urls
    /// </summary>
    public Dictionary<string, Uri> Designers { get; set; }
    /// <summary>
    /// The dictionary of artists' names and urls
    /// </summary>
    public Dictionary<string, Uri> Artists { get; set; }
    /// <summary>
    /// The translator credits string
    /// </summary>
    public string TranslatorCredits { get; set; }

    /// <summary>
    /// Constructs an AppInfo
    /// </summary>
    internal AppInfo()
    {
        ID = "";
        Name = "";
        Description = "";
        ShortName = "";
        Version = "0.0.0";
        Changelog = "";
        SourceRepo = new Uri("about:blank");
        IssueTracker = new Uri("about:blank");
        SupportUrl = new Uri("about:blank");
        ExtraLinks = new Dictionary<string, Uri>();
        Developers = new Dictionary<string, Uri>();
        Designers = new Dictionary<string, Uri>();
        Artists = new Dictionary<string, Uri>();
        TranslatorCredits = "";
    }

    /// <summary>
    /// Gets whether or not the application version is a development version
    /// </summary>
    /// <returns>True for development version, else false</returns>
    public bool GetIsDevelVersion() => Version.Contains("-");
    
    /// <summary>
    /// Convert changelog to HTML format
    /// </summary>
    /// <returns>HTML string</returns>
    public string HTMLChangelog
    {
        get
        {
            var result = new StringBuilder();
            foreach (var line in Changelog.Trim().Split(Environment.NewLine))
            {
                result.Append(line.Trim());
                result.Append(Environment.NewLine);
            }
            return Markdown.ToHtml(result.ToString());
        }
    }

    /// <summary>
    /// Convert dictionary with urls to string array
    /// </summary>
    /// <param name="dict">Dictionary with urls</param>
    /// <returns>String array</returns>
    public string[] ConvertURLDictToArray(Dictionary<string, Uri> dict)
    {
        var list = new List<string>();
        foreach (var pair in dict)
        {
            list.Add($"{pair.Key} {pair.Value}");
        }
        return list.ToArray();
    }
}