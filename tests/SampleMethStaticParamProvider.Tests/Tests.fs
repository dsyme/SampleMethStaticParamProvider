#if INTERACTIVE
#r @"..\..\bin\SampleMethStaticParamProvider.dll"
#r @"..\..\packages\NUnit.2.6.3\lib\nunit.framework.dll"
#else
module SampleMethStaticParamProvider.Tests
#endif

open ExampleTypeProvider
open NUnit.Framework

[<Test>]
let ``basic test`` () =

    let x = ExampleType()

    let v1 = x.ExampleMethWithStaticParam<1>(1) 
    let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
    let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 


    Assert.AreEqual(v1,1)

    Assert.AreEqual(v2,2)

    Assert.AreEqual(v3,3)
