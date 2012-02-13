using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using FluentBuild.Core;
using FluentBuild.Utilities;

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
            var assemblyInfoFile = new BuildArtifact( Projects.Dwolla.OutputDll + ".AssemblyInfo.cs" );
            Projects.Dwolla.AssemblyInfo.OutputTo( assemblyInfoFile );

            Build.UsingCsc
                .Target.Library
                .IncludeDebugSymbols
                .AddRefences( Folders.Lib.Files("*.dll").Files.ToArray() )
                .AddSources( new FileSet().Include( Projects.Dwolla.Folder ).RecurseAllSubDirectories.Filter( "*.cs" ) )
                .AddSources( new FileSet().Include( assemblyInfoFile ) )
                .OutputFileTo( Projects.Dwolla.OutputDll )
                .Execute();

            BuildUtil.GetProjectReferences( Projects.Dwolla.ProjectFile, Folders.Lib )
                .Copy.To( Folders.CompileOutput );

            assemblyInfoFile.Delete( OnError.Continue );

            //Run.ILMerge
            //    .AddSource( Projects.Dwolla.OutputDll )
            //    .OutputTo( Folders.CompileOutput.File( "Dwolla.merge.dll" ) )
            //    .Execute();

            //Folders.CompileOutput.Files( "*.dll" )
            //    .Files.Aggregate( Run.ILMerge, ( merge, s ) => merge.AddSource( s ), merge => merge )
            //    .OutputTo( "Dwolla.merge.dll" )
            //    .Execute();

            Run.Zip.Compress.SourceFolder( Folders.CompileOutput )
                .To( Projects.Dwolla.Package );
        }
    }
}