using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Fluent.IO;
using FluentBuild;
using FluentFs.Core;

namespace BuildFiles.Tasks
{
    public class BuildTask : BuildFile
    {
        public BuildTask()
        {
            this.AddTask( "clean", Clean );
            this.AddTask( "build", CompileSources );
        }

        public void Clean()
        {
            Folders.CompileOutput.Wipe();
            Folders.Package.Wipe();
        }

        public void CompileSources()
        {
            var assemblyInfoFile = Folders.CompileOutput.File( "Global.AssemblyInfo.cs" );

            Task.CreateAssemblyInfo.Language.CSharp( aid =>
                {
                    Projects.DwollaCheckout.AssemblyInfo( aid );
                    aid.OutputPath( assemblyInfoFile );
                } );

            Task.Build.MsBuild( msb =>
                {
                    msb.Configuration( "Release" )
                       .ProjectOrSolutionFilePath( Projects.DwollaCheckout.ProjectFile )
                       .AddTarget( "Rebuild" )
                       .OutputDirectory( Projects.DwollaCheckout.OutputDirectory );
                });

            assemblyInfoFile.Delete( OnError.Continue );

            Defaults.Logger.WriteHeader( "BUILD COMPLETE. Packaging ..." );

            
            //copy compile directory to package directory
            Path.Get( Projects.DwollaCheckout.OutputDirectory.ToString() )
                .Copy( Projects.DwollaCheckout.PackageDir.ToString(), Overwrite.Always, recursive: true );

            var version = Properties.CommandLineProperties.Version();

            Defaults.Logger.Write( "RESULTS", "NuGet packing" );

            var nuget = Path.Get( Folders.Lib.ToString() )
                .Files( "NuGet.exe", recursive: true ).First();

            Task.Run.Executable( e => e.ExecutablePath(nuget.FullPath)
                .WithArguments( "pack", Projects.DwollaCheckout.NugetSpec.Path, "-Version", version, "-OutputDirectory", Folders.Package.ToString() ) );

            Defaults.Logger.Write( "RESULTS", "Setting NuGet PUSH script" );
            var pushcmd = "{0} push {1}".With( nuget.MakeRelative().ToString(), Path.Get(Projects.DwollaCheckout.NugetNupkg.ToString()).MakeRelative().ToString() );
            //Defaults.Logger.Write( "RESULTS", pushcmd );
            System.IO.File.WriteAllText( "nuget.push.bat", pushcmd );
        }

    }
}