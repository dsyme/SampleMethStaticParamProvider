namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("SampleMethStaticParamProvider")>]
[<assembly: AssemblyProductAttribute("SampleMethStaticParamProvider")>]
[<assembly: AssemblyDescriptionAttribute("SampleMethStaticParamProvider")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
