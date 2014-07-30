# Sample of a type provider where methods have static parameters (demo of an F# 4.0+ feature)

This is a Hackathon project to add the F# language feature described here: http://fslang.uservoice.com/forums/245727-f-language/suggestions/6097685-allow-static-arguments-to-type-provider-methods-e.


The core feature is implemented to prototype quality. You can see the feature here: https://visualfsharp.codeplex.com/SourceControl/network/forks/dsyme/cleanup/contribution/7203 


## Feature Design

From the point of view of the programmer, the feature lets them use type provider where methods can take static parameters:

    let x = ExampleType()

    let v1 = x.ExampleMethWithStaticParam<1>(1) 
    let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
    let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 

Note that the sample method changes its number of static parameters with the static parameter given to the method. The return type can likewise be changed (and can be a provided type whose name depends on the input parameter)


From the point of view of the type provider author, using the modified ProvideTypes API, provided methods can now include a DefineStaticParameters specification:

    let staticParams = [ProvidedStaticParameter("Count", typeof<int>)]
    let exampleMethWithStaticParams =  
      let m = ProvidedMethod("ExampleMethWithStaticParam", [ ], typeof<int>, IsStaticMethod = false)
      m.DefineStaticParameters(staticParams, (fun nm args ->
          let arg = args.[0] :?> int
          let parms = [ for i in 1 .. arg -> ProvidedParameter("arg" + string i, typeof<int>)]
          let m2 = 
              ProvidedMethod(nm, parms, typeof<int>, IsStaticMethod = false,
                             InvokeCode = fun args -> <@@ arg @@>)
          newType.AddMember m2
          m2))
      m


The design is very much like that for ``DefineStaticParameters`` on types.

Notes:

- Here `nm` is the name to use for the actual instantiated method. It is a mangled name
- - The arguments and the return type on are irrelevant.  
- The staticness of  ``exampleMethWithStaticParams`` is relevant and must match the staticness of the instantiated method ``m2``. This is not checked.
- The method ``m2`` must be added to the same type as ``exampleMethWithStaticParams``


Underneath the type provider API is extended artificially through a pattern where we use reflection to interrogate the type provider object for an additional pair of methods, see here: https://visualfsharp.codeplex.com/SourceControl/network/forks/dsyme/cleanup/contribution/7203#!/tab/changes.

We don't use additions to the ITypeProvider interface in the protoype to avoid building a new FSharp.Core.dll, and even in the product we would still allow an idiom-based acess to type providers to avoid type providers needing to take a dependency on the latest FSharp.Core.dll (whcih can easily cause cascading problems).


## Example things we might now build:


1. A modified CSV type provider that lets you do add a column. The return type would be a _new_ object representing the data collection with the column added.  This is a bit like a "reccord calculus" where you can add and remove columns in user code in strongly typed ways (but can't write code that is generic over column types)

       // assume csvData has some type 

       type MyCsvFile = FSharp.Data.CsvProvider<"mycsv.csv">
       
       let csvData = MyCsvFile.LoadSample() // ....
       
       [ for row in csvData -> row.Column1, row.Column2 ]
       
       let newCsvData = csvData.WithColumn<"Column3", "int">()  
       
       [ for row in newCsvData -> row.Column1, row.Column2, row.Column3 ]
       
       let newCsvData2 = newCsvData.WithColumn<"Column4", "int">()  
       
       [ for row in newCsvData2 -> row.Column1, row.Column2, row.Column3, row.Column4 ]
       
       let newCsvData3 = newCsvData.RemoveColumn<"Column3", "int">()  
       
       [ for row in newCsvData2 -> row.Column1, row.Column2, row.Column4 ] // can't access Column3 anymore!

2. A regex type provider that lets you do this:

       RegEx.Parse<"a+b*c?">(data)
       
       RegEx.Match<"a+b*c?">(data)
       
       RegEx.IsMatch<"a+b*c?">(data)


2. A more strongly typed data frame library that lets you add/remove columns in a strongly typed functional way, like the CSV example.

3. A modified SqlClient type provider that takes the SQL command? - see [the original proposal](http://fslang.uservoice.com/forums/245727-f-language/suggestions/6097685-allow-static-arguments-to-type-provider-methods-e)


      type SqlConnect = SomeSQlProvider<"Some ConnectionString">

      let ctxt = SqlConnect.GetDataContext()
    
      let sqlTable = ctxt.CreateTable<"CREATE TABLE Foo COLUMNS X, Y">()  


4. A search functionality in the DbPedia provider which reveals individual entities:

      type DbPedia = DbPediaProvider<"Some Parameters">

      let ctxt = DbPedia.GetDataContext()
    
      let princeTheMusician = ctxt.Ontology.People.Search<"Prince">. // At this point the completions for all people matching "Prince" would be shown

## Can this be done today?

Sort of. Things generally get very nasty when you have a primary set of static parameters on a type, but some of the methods naturally take static parameters. With static parameters on provided methods you get to do something like this (at least, if things are working correctly)

      type DbPediaProvider<”A”>
                method Search<”B1”> (returns types depending on a search of DbPedia using string "B1")
                method Search<”B2”> (returns types depending on a search of DbPedia using string "B2")
                nested type SearchResults
                    nested type Search_B1_Results // use the mangled name provided for the method to create these types
                    nested type Search_B2_Results // use the mangled name provided for the method to create these types

Previously when you could only parameterize types doing this sort of thing was much more painful.







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



#### Now get and build the sample provider (this repo)

    git clone https://github.com/dsyme/SampleMethStaticParamProvider


This type provider that uses the new feature.  This is in a separate repo and *you will need to update the path to the F# compiler [here](https://github.com/dsyme/SampleMethStaticParamProvider/blob/master/tests/SampleMethStaticParamProvider.Tests/SampleMethStaticParamProvider.Tests.fsproj#L14)*

The sample provider lets you do this:

    let x = ExampleType()

    let v1 = x.ExampleMethWithStaticParam<1>(1) 
    let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
    let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 

Note that the sample method changes its number of static parameters with the static parameter given to the method.


