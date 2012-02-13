using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using FluentBuild.Core;

namespace BuildFiles
{
    public class BuildUtil
    {
        public static FileSet GetProjectReferences(BuildArtifact projectFile, BuildFolder libFolder)
        {
            var references = XDocUtil.LoadIgnoreingNamespace( projectFile.ToString() )
                .XPathSelectElements( "//HintPath" )
                .Select( h => Path.GetFileNameWithoutExtension( h.Value ) )
                .ToList();


            return references.Aggregate( new FileSet(),
                                         ( set, assembly ) =>
                                             {
                                                 Folders.Lib.Files( "*{0}*".With( assembly ) )
                                                     .Files.ToList()
                                                     .ForEach( f => set.Include( f ) );

                                                 return set;
                                             }, set => set );
        }
    }
}