//#if INTERACTIVE
//open System
//Environment.CurrentDirectory <- workingDir
//#else
//#endif

// include Fake lib
#I @"../packages/build/FAKE/tools"
#I @"../packages/build/DotNetZip/lib/net20"
#r @"FakeLib.dll"
#r @"DotNetZip.dll"

#load @"Utils.fsx"

open Fake
open Utils
open System.Reflection
open Helpers

let workingDir = ChangeWorkingFolder();

trace (sprintf "WORKING DIR: %s" workingDir)

let ProjectName = "Dwolla.Checkout";
let GitHubUrl = "https://github.com/bchavez/Dwolla"

let Folders = Setup.Folders(workingDir)
let Files = Setup.Files(Folders)
let Projects = Setup.Projects(ProjectName, Folders)

let DwollaProject = NugetProject("Dwolla.Checkout", "Dwolla Checkout API for .NET", Folders)
let TestProject = TestProject("Dwolla.Checkout.Tests", Folders)



Target "msb" (fun _ ->
    
    let tag = "msb_build";

    let buildProps = [ 
                        
                     ]

    !! DwollaProject.ProjectFile
    |> MSBuildReleaseExt (DwollaProject.OutputDirectory @@ tag) buildProps "Build"
    |> Log "AppBuild-Output: "


    !! TestProject.ProjectFile
    |> MSBuild "" "Build" (("Configuration", "Debug")::buildProps)
    |> Log "AppBuild-Output: "
)



Target "restore" (fun _ -> 
     trace "MS NuGet Project Restore"
     Projects.SolutionFile
     |> RestoreMSSolutionPackages (fun p ->
            { p with OutputPath = (Folders.Source @@ "packages" )}
        )
 )

Target "nuget" (fun _ ->
    trace "NuGet Task"
    
    let releaseNotes = History.NugetText Files.History GitHubUrl

    let dwollaConfig = NuGetConfig DwollaProject Folders Files     
    let finalConfig =
        {dwollaConfig with
                      ReleaseNotes = releaseNotes
                      Files =  ( ( sprintf "**/%s**" DwollaProject.Name) , Some ("lib"@@"net40"), None )
                                ::dwollaConfig.Files
        }
    
    NuGet ( fun p -> finalConfig) DwollaProject.NugetSpec
//    DotnetPack DwollaProject Folders.Package
)

Target "push" (fun _ ->
    trace "NuGet Push Task"
    
    failwith "Only CI server should publish on NuGet"
)



Target "zip" (fun _ -> 
    trace "Zip Task"

    !!(DwollaProject.OutputDirectory @@ "**") |> Zip Folders.CompileOutput (Folders.Package @@ DwollaProject.Zip)
)

open AssemblyInfoFile

let MakeAttributes () =
    let attrs = [
                    Attribute.Description GitHubUrl
                    Attribute.InternalsVisibleTo(TestProject.Name)
                ]
    attrs


Target "BuildInfo" (fun _ ->
    
    trace "Writing Assembly Build Info"

    MakeBuildInfo DwollaProject Folders (fun bip -> 
        { bip with
            ExtraAttrs = MakeAttributes() } )

//    JsonPoke "version" BuildContext.FullVersion BogusProject.ProjectJson

    //JsonPoke "packOptions.releaseNotes" releaseNotes BogusProject.ProjectJson
)


Target "Clean" (fun _ ->
    DeleteFile Files.TestResultFile
    CleanDirs [Folders.CompileOutput; Folders.Package]

//    JsonPoke "version" "0.0.0-localbuild" BogusProject.ProjectJson
//    JsonPoke "packOptions.releaseNotes" "" BogusProject.ProjectJson
//    JsonPoke "buildOptions.keyFile" "" BogusProject.ProjectJson

    MakeBuildInfo DwollaProject Folders (fun bip ->
         {bip with
            DateTime = System.DateTime.Parse("1/1/2015")
            ExtraAttrs = MakeAttributes() } )

)

let RunTests() =
    CreateDir Folders.Test
    let nunit = findToolInSubPath "nunit-console.exe" Folders.Lib
    let nunitFolder = System.IO.Path.GetDirectoryName(nunit)

    !! TestProject.TestAssembly
    |> NUnit (fun p -> { p with 
                            ToolPath = nunitFolder
                            OutputFile = Files.TestResultFile
                            ErrorLevel = TestRunnerErrorLevel.Error }) 


open Fake.AppVeyor

Target "ci" (fun _ ->
    trace "ci Task"
)

Target "test" (fun _ ->
    trace "TEST"
    RunTests()
)

Target "citest" (fun _ ->
    trace "CI TEST"
    RunTests()
    UploadTestResultsXml TestResultsType.NUnit Folders.Test
)



"Clean"
    ==> "restore"
    ==> "BuildInfo"
        

//build systems, order matters
"BuildInfo"
    ==> "zip"

"BuildInfo"
    ==> "msb"
    ==> "zip"

"BuildInfo"
    ==> "zip"

"msb"
    ==> "nuget"


"nuget"
    ==> "ci"

"nuget"
    ==> "push"

"zip"
    ==> "ci"


//test task depends on msbuild
"msb"
    ==> "test"



// start build
RunTargetOrDefault "msb"
