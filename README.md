# Sample of a type provider with static parameters

### Get the repo

    git clone -b staticparams https://git01.codeplex.com/forks/dsyme/cleanup staticparams
    cd staticparams

#### Build an updated F# compiler and F# interactive as usual for the Visual F# Team open source build 


    cd src
    gacutil /i ..\lkg\FSharp-2.0.50726.900\bin\FSharp.Core.dll
    msbuild fsharp-proto-build.proj
    msbuild fsharp-library-build.proj /p:TargetFramework=net40
    msbuild fsharp-compiler-build.proj /p:TargetFramework=net40

Note: no Visual Tools are built as yet, so the new feature is only accessible to command-line tools. You should be able to follow the instructions on http://visualfsharp.codeplex.com to build, install and use the tools, though setting FscToolPath may still be needed, see below.


#### Now get and build the sample provider 

    git clone https://github.com/dsyme/SampleMethStaticParamProvider

This type provider that uses this.  This is in a separate repo and you will need to update the path to the F# compiler [here](https://github.com/dsyme/SampleMethStaticParamProvider/blob/master/tests/SampleMethStaticParamProvider.Tests/SampleMethStaticParamProvider.Tests.fsproj#L14)

The sample provider lets you do this:

    let x = ExampleType()

    let v1 = x.ExampleMethWithStaticParam<1>(1) 
    let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
    let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 

Note that the sample method changes its number of static parameters with the static parameter given to the method.


Example things we might now build:


1. A modified CSV type provider that lets you do add a column. The return type would be a _new_ object representing the data collection with the column added.  This is a bit like a "reccord calculus" where you can add and remove columns in user code in strongly typed ways (but can't write code that is generic over column types)

    // assume csvData has some type 
    
    csvData.WithColumnInit<"Row Number", "int">(fun i otherValues -> string i) // add a column using an initializer
    


2. A regex type provider that lets you do this:

    RegEx.Parse<"a+b*c?">(data)
    
    RegEx.Match<"a+b*c?">(data)
    
    RegEx.IsMatch<"a+b*c?">(data)


2. A more strongly typed data frame library that lets you add/remove columns in a strongly typed functional way, like the CSV example.

3. A modified SqlClient type provider that takes the SQL command? - see [the original proposal](http://fslang.uservoice.com/forums/245727-f-language/suggestions/6097685-allow-static-arguments-to-type-provider-methods-e)


4. A search functionality in the DbPedia provider which reveals individual entities:


Things generally get more interesting when you have a primary set of static parameters on a type, but some of the methods naturally take static parameters. With static parameters on provided methods you get to do something like this (at least, if things are working correctly)

    type DbPediaProvider<”A”>
                method Search<”B1”> (returns types depending on a search of DbPedia using string "B1")
                method Search<”B2”> (returns types depending on a search of DbPedia using string "B2")
                nested type SearchResults
                    nested type Search_B1_Results // use the mangled name provided for the method to create these types
                    nested type Search_B2_Results // use the mangled name provided for the method to create these types

Previously when you could only parameterize types doing this sort of thing was much more painful.






