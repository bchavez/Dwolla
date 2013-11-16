using System.Linq;
using FluentBuild;
using FluentBuild.Core;
using FluentFs.Core;
using System.Linq;
using ILMerging;

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
            Folders.PackageOutput.Wipe();
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

            Task.Run.Zip.Compress( z =>
                {
                    z.SourceFolder( Projects.DwollaCheckout.OutputDirectory );
                    z.To( Projects.DwollaCheckout.Package );
                } );

            Defaults.Logger.Write( "RESULTS", "{0}", Projects.DwollaCheckout.Package.ToString() );

            
            //IL MERGE

        }

    }
}