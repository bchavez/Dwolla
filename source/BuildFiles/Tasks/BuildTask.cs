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

            Defaults.Logger.WriteHeader( "BUILD COMPLETE. Packaging ..." );

            Task.Run.Zip.Compress( z =>
                {
                    z.SourceFolder( Projects.Dwolla.OutputDirectory );
                    z.To( Projects.Dwolla.Package );
                } );

            Defaults.Logger.Write( "RESULTS", "{0}", Projects.Dwolla.Package.ToString() );

            
            //IL MERGE
            Projects.Dwolla.ILMergeDirectory.Create();
            var ilMerge = new ILMerge();
            ilMerge.SetInputAssemblies( Projects.Dwolla.OutputDirectory.Files( "**/*.dll" ).Files.ToArray() );
            ilMerge.OutputFile = Projects.Dwolla.ILMergeFile.ToString();
            ilMerge.TargetKind = ILMerge.Kind.Dll;
            ilMerge.Internalize = true;

            ilMerge.SetTargetPlatform( "v4", Defaults.FrameworkVersion.GetPathToFrameworkInstall() );
            ilMerge.DebugInfo = true;
            ilMerge.Merge();


            Task.Run.Zip.Compress( z =>
                {
                    z.SourceFolder( Projects.Dwolla.ILMergeDirectory );
                    z.To( Projects.Dwolla.ILMergePackage );
                } );

            Defaults.Logger.Write( "RESULTS", "{0}", Projects.Dwolla.ILMergePackage.ToString() );
        }

    }
}