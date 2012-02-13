using FluentBuild.ApplicationProperties;
using FluentBuild.Core;
using FluentBuild.Utilities;

namespace BuildFiles
{
    public static class ExtensionsForCommandLineProperties
    {
        public static string Version(this CommandLineProperties p)
        {
            return System.Version.Parse( p.GetProperty( "Version" ) ).ToString();
        }
    }
    public static class ExtensionsForString
    {
        public static string With( this string format, params object[] args )
        {
            return string.Format( format, args );
        }
    }

    public static class ExtensionsForBuildFolders
    {
        public static BuildFolder Wipe(this BuildFolder f)
        {
            return f.Delete( OnError.Continue ).Create();
        }
    }
}