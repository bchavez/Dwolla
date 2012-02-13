﻿using System;
using FluentBuild.AssemblyInfoBuilding;
using FluentBuild.Core;

namespace BuildFiles
{
    public class Folders
    {
        public static readonly BuildFolder WorkingFolder = new BuildFolder( Properties.CurrentDirectory );
        public static readonly BuildFolder CompileOutput = WorkingFolder.SubFolder( "__compile" );
        public static readonly BuildFolder PackageOutput = WorkingFolder.SubFolder( "__package" );
        public static readonly BuildFolder Lib = WorkingFolder.SubFolder( "lib" );
        public static readonly BuildFolder Source = WorkingFolder.SubFolder( "source" );
        public static readonly BuildFolder Tools = WorkingFolder.SubFolder( "tools" );
    }

    public class Projects
    {
        private static AssemblyInfoDetails GlobalAssemblyInfo()
        {
            return AssemblyInfo.Language.CSharp
                .Company( "Bit Armory, Inc." )
                .Copyright( "Copyright Bit Armory, Inc © " + DateTime.UtcNow.Year )
                .Version( Properties.CommandLineProperties.Version() )
                .FileVersion( Properties.CommandLineProperties.Version() )
                .InformationalVersion( Properties.CommandLineProperties.Version() );
        }

        public class Dwolla
        {
            public static readonly BuildFolder Folder = Folders.Source.SubFolder( "Dwolla" );
            public static readonly BuildArtifact ProjectFile = Folder.File( "Dwolla.csproj" );
            public static readonly BuildArtifact OutputDll = Folders.CompileOutput.File( "Dwolla.dll" );
            public static readonly BuildArtifact Package = Folders.PackageOutput.File( "Dwolla-{0}.zip".With( Properties.CommandLineProperties.Version() ) );

            public static readonly AssemblyInfoDetails AssemblyInfo =
                GlobalAssemblyInfo()
                    .Title( "Dwolla API for .NET" )
                    .Product( "Dwolla API" )
                    .Description( "A .NET implementation for the Dwolla API." );
        }

        public class Tests
        {
            public static readonly BuildFolder Folder = Folders.Source.SubFolder( "Dwolla.Tests" );
        }
    }


}