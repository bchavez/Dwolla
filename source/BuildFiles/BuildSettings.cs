using System;
using FluentBuild;
using FluentBuild.AssemblyInfoBuilding;
using FluentFs.Core;

namespace BuildFiles
{
    public class Folders
    {
        public static readonly Directory WorkingFolder = new Directory( Properties.CurrentDirectory );
        public static readonly Directory CompileOutput = WorkingFolder.SubFolder( "__compile" );
        public static readonly Directory Package = WorkingFolder.SubFolder( "__package" );
        public static readonly Directory Source = WorkingFolder.SubFolder( "source" );

        public static readonly Directory Lib = Source.SubFolder( "packages" );
        public static readonly Directory Tools = WorkingFolder.SubFolder( "tools" );
    }

    public class Projects
    {
        private static void GlobalAssemblyInfo(IAssemblyInfoDetails aid)
        {
            aid.Company( "Brian Chavez" )
               .Copyright( "Brian Chavez © " + DateTime.UtcNow.Year )
               .Version( Properties.CommandLineProperties.Version() )
               .FileVersion( Properties.CommandLineProperties.Version() )
               .InformationalVersion( "{0} built on {1} UTC".With( Properties.CommandLineProperties.Version(), DateTime.UtcNow ) )
               .Trademark( "MIT License" )
               .Description( "http://www.github.com/bchavez/Dwolla" );
        }

        public static readonly File SolutionFile = Folders.Source.File( "Dwolla.Checkout.sln" );

        public class DwollaCheckout
        {
            public static readonly Directory Folder = Folders.Source.SubFolder( "Dwolla.Checkout" );
            public static readonly File ProjectFile = Folder.File( "Dwolla.Checkout.csproj" );
            public static readonly Directory OutputDirectory = Folders.CompileOutput.SubFolder( "Dwolla.Checkout" );
            public static readonly File OutputDll = OutputDirectory.File( "Dwolla.Checkout.dll" );
            public static readonly Directory PackageDir = Folders.Package.SubFolder( "Dwolla.Checkout" );
            
            public static readonly File NugetSpec = Folders.Source.SubFolder(".nuget").File( "Dwolla.Checkout.nuspec" );
            public static readonly File NugetNupkg = Folders.Package.File( "Dwolla.Checkout.{0}.nupkg".With( Properties.CommandLineProperties.Version() ) );

            public static readonly Action<IAssemblyInfoDetails> AssemblyInfo =
                i =>
                    {
                        i.Title( "Dwolla Checkout API for .NET" )
                         .Product( "Dwolla Checkout API" );

                        GlobalAssemblyInfo( i );
                    };
        }

        public class Tests
        {
            public static readonly Directory Folder = Folders.Source.SubFolder( "Dwolla.Checkout.Tests" );
        }
    }


}