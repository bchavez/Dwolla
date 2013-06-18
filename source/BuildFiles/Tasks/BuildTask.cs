using System.Linq;
using FluentBuild;
using FluentBuild.Core;
using FluentFs.Core;
using System.Linq;

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
                    Projects.Dwolla.AssemblyInfo( aid );
                    aid.OutputPath( assemblyInfoFile );
                } );

            Task.Build.MsBuild( msb =>
                {
                    msb.Configuration( "Release" )
                       .ProjectOrSolutionFilePath( Projects.Dwolla.ProjectFile )
                       .AddTarget( "Rebuild" )
                       .OutputDirectory( Projects.Dwolla.OutputDirectory );
                });

            assemblyInfoFile.Delete( OnError.Continue );

            Task.Run.Zip.Compress( z =>
                {
                    z.SourceFolder( Projects.Dwolla.OutputDirectory );
                    z.To( Projects.Dwolla.Package );
                } );

            //IL MERGE

            Task.Run.ILMerge( il =>
                {
                    var exe = Folders.Lib.Files( @"**\ILMerge.exe" ).Files.First();

                    il.ExecutableLocatedAt( exe );

                    //the dlls to merge
                    Projects.Dwolla.OutputDirectory.Files( "**/*.dll" )
                            .Files.ToList()
                            .ForEach( dll => il.AddSource( dll ) );

                    Projects.Dwolla.ILMergeDirectory.Create();
                    //output.
                    il.OutputTo( Projects.Dwolla.ILMergeFile );

                } );

            Task.Run.Zip.Compress( z =>
                {
                    z.SourceFolder( Projects.Dwolla.ILMergeDirectory );
                    z.To( System.IO.Path.ChangeExtension( Projects.Dwolla.Package.ToString(), ".ILMerged.zip" ) );
                } );

            Defaults.Logger.WriteHeader( "BUILD COMPLETE" );
            Defaults.Logger.Write( "RESULTS", "{0}", Projects.Dwolla.Package.ToString() );
        }
    }
}