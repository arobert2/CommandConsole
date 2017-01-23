using System;

namespace CommandConsole
{
    public partial class LoadApps
    {
        partial void LoadLibrary()
        {
            AppsToLoad.Library.Add(new SimpleWebServer().Keyword, new SimpleWebServer());
        }
    }
}