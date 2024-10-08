using NokoWebApiSdk.Extensions.Scalar.Enums;
using NokoWebApiSdk.Extensions.Scalar.Options;
using NokoWebApiSdk.Extensions.Scalar.Schemas;

namespace NokoWebApiSdk.Extensions.Scalar.Mapper;

internal static class ScalarOptionsMapper
{
    internal static readonly Dictionary<ScalarTarget, ScalarClient[]> ClientOptions = new()
    {
        { ScalarTarget.C, [ScalarClient.Libcurl] },
        { ScalarTarget.Clojure, [ScalarClient.CljHttp] },
        { ScalarTarget.CSharp, [ScalarClient.HttpClient, ScalarClient.RestSharp] },
        { ScalarTarget.Http, [ScalarClient.Http11] },
        { ScalarTarget.Java, [ScalarClient.AsyncHttp, ScalarClient.NetHttp, ScalarClient.OkHttp, ScalarClient.Unirest] },
        { ScalarTarget.JavaScript, [ScalarClient.Xhr, ScalarClient.Axios, ScalarClient.Fetch, ScalarClient.JQuery] },
        { ScalarTarget.Node, [ScalarClient.Undici, ScalarClient.Native, ScalarClient.Request, ScalarClient.Unirest, ScalarClient.Axios, ScalarClient.Fetch] },
        { ScalarTarget.ObjC, [ScalarClient.Nsurlsession] },
        { ScalarTarget.OCaml, [ScalarClient.CoHttp] },
        { ScalarTarget.Php, [ScalarClient.Curl, ScalarClient.Guzzle, ScalarClient.Http1, ScalarClient.Http2] },
        { ScalarTarget.PowerShell, [ScalarClient.WebRequest, ScalarClient.RestMethod] },
        { ScalarTarget.Python, [ScalarClient.Python3, ScalarClient.Requests] },
        { ScalarTarget.R, [ScalarClient.Httr] },
        { ScalarTarget.Ruby, [ScalarClient.Native] },
        { ScalarTarget.Shell, [ScalarClient.Curl, ScalarClient.Httpie, ScalarClient.Wget] },
        { ScalarTarget.Swift, [ScalarClient.Nsurlsession] },
        { ScalarTarget.Go, [ScalarClient.Native] },
        { ScalarTarget.Kotlin, [ScalarClient.OkHttp] }
    };

    internal static ScalarConfiguration ToScalarConfiguration(this ScalarOptions options)
    {
        return new ScalarConfiguration
        {
            Proxy = options.ProxyUrl,
            Theme = options.Theme.ToString(),
            Favicon = options.Favicon,
            DarkMode = options.DarkMode,
            HideModels = options.HideModels,
            HideDarkModeToggle = options.HideDarkModeToggle,
            HideDownloadButton = options.HideDownloadButton,
            HideTestRequestButton = options.HideTestRequestButton,
            DefaultOpenAllTags = options.DefaultOpenAllTags,
            ForceDarkModeState = options.ForceThemeMode?.ToString(),
            ShowSidebar = options.ShowSidebar,
            DefaultFonts = options.WithDefaultFonts,
            CustomCss = options.CustomCss,
            SearchHotKey = options.SearchHotKey,
            Servers = options.Servers,
            Metadata = options.Metadata,
            Authentication = options.Authentication,
            TagSorter = options.TagSorter?.ToString(),
            HiddenClients = GetHiddenClients(options),
            DefaultHttpClient = new DefaultHttpClient
            {
                ClientKey = options.DefaultHttpClient.Value.ToString(),
                TargetKey = options.DefaultHttpClient.Key.ToString()
            }
        };
    }

    private static Dictionary<string, IEnumerable<string>>? GetHiddenClients(ScalarOptions options)
    {
        var targets = ProcessOptions(options);

        return targets?.ToDictionary(k =>
                k.Key.ToString(),
            k => k.Value.Select(v => v.ToString())
        );
    }

    private static Dictionary<ScalarTarget, ScalarClient[]>? ProcessOptions(ScalarOptions options)
    {
        if (options.HiddenClients)
        {
            return ClientOptions;
        }

        if (options.EnabledTargets.Length == 0 && options.EnabledClients.Length == 0)
        {
            return null;
        }

        var selected = new Dictionary<ScalarTarget, ScalarClient[]>();
        foreach (var item in ClientOptions)
        {
            if (options.EnabledTargets.Length != 0 &&
                !options.EnabledTargets.Contains(item.Key))
            {
                selected.Add(item.Key, item.Value);
                continue;
            }

            if (options.EnabledClients.Length == 0)
            {
                continue;
            }

            var clients = item.Value
                .Where(client => !options.EnabledClients.Contains(client))
                .ToArray();

            if (clients.Length != 0)
            {
                selected.Add(item.Key, clients);
            }
        }
        
        return selected;
    }
}