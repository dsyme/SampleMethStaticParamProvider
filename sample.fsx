
#r @"C:\GitHub\dsyme\SampleMethStaticParamProvider\bin\SampleMethStaticParamProvider.dll"


open ExampleTypeProvider

let x = ExampleType()

x.ExampleMethWithStaticParam<1>(1) |> printfn "%d"
x.ExampleMethWithStaticParam<2>(1,2) |> printfn "%d"
x.ExampleMethWithStaticParam<3>(1,2,3) |> printfn "%d"


