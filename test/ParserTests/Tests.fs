namespace ParserTests

open AST
open Combinator
open Parser
open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestMethodPassing () =
        Assert.IsTrue(true);



    [<TestMethod>]
    member this.NumberParserTest () =
        let input = prepare "123"
        let result = numberParser input
        match result with
        | Success(value, _) -> Assert.AreEqual(123, value)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse an integer.")



    [<TestMethod>]
    member this.GOLDParserTest () =
        let input = prepare "gold"
        let result = GOLDparser input
        printfn "%A" result
        match result with
        | Success(value, _) -> Assert.AreEqual("gold", value)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse 'gold'.")

    [<TestMethod>]
    member this.SLVRParserTest () =
        let input = prepare "slvr"
        let result = SLVRParser input
        match result with
        | Success(value, _) -> Assert.AreEqual("slvr", value)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse 'slvr'.")


    [<TestMethod>]
    member this.TSLAParserTest () =
        let input = prepare "tsla"
        let result = TSLAParser input
        match result with
        | Success(value, _) -> Assert.AreEqual("tsla", value)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse 'tsla'.")

    [<TestMethod>]
    member this.BuyParserTest () =
        let input = prepare "buy(gold,10,2023)"
        let result = buyParser input
        match result with
        | Success(Command.BuyCommand(value), _) ->
            Assert.AreEqual("gold", value.stock)
            Assert.AreEqual(10.0, value.buy)
            Assert.AreEqual(2023, value.year) 
        | Failure(_, _) -> Assert.Fail("Parser failed to parse buy command.")
        | _ -> Assert.Fail("Unexpected result type.")



    [<TestMethod>]
    member this.SellParserTest () =
        let input = prepare "sell(tsla,5,2024)"
        let result = sellParser input
        match result with
        | Success(Command.SellCommand(value), _) ->
            Assert.AreEqual("tsla", value.stock)
            Assert.AreEqual(5.0, value.sell)
            Assert.AreEqual(2024, value.year)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse sell command.")
        | _ -> Assert.Fail("Unexpected result type.")


    [<TestMethod>]
    member this.BargraphOutputParsersTest () =
        let bargraphInput = prepare "bargraph,2023"
        let bargraphResult = bargraphParser bargraphInput
        match bargraphResult with
        | Success(Bargraph, _) -> Assert.IsTrue(true)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse bargraph output.")
        | _ -> Assert.Fail("Unexpected result type.")

    [<TestMethod>]
    member this.TimeSeriesOutputParsersTest () =
        let timeseriesInput = prepare "timeseries,2024"
        let timeseriesResult = timeseriesParser timeseriesInput
        match timeseriesResult with
        | Success(Timeseries, _) -> Assert.IsTrue(true)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse timeseries output.")
        | _ -> Assert.Fail("Unexpected result type.")

    [<TestMethod>]
    member this.PortfolioOutputParsersTest () =
        let portfolioInput = prepare "portfolio,2025"
        let portfolioResult = portfolioParser portfolioInput
        match portfolioResult with
        | Success(Portfolio, _) -> Assert.IsTrue(true)
        | Failure(_, _) -> Assert.Fail("Parser failed to parse portfolio output.")
        | _ -> Assert.Fail("Unexpected result type.")




    [<TestMethod>]
    member this.EntireParseMethodTest () =
        let programInput = "buy(gold,10,2023)sell(tsla,5,2024)addcapital(1000,2023)output(bargraph,2023)"
        let result = parse programInput
        match result with
        | Some(ast) -> 
            match ast with
            | [Command(BuyCommand(buyCmd)); Command(SellCommand(sellCmd)); 
            Command(AddCapitalCommand(addCapitalCmd)); Output(Bargraph)] ->
                // Verify the individual commands
                Assert.AreEqual("gold", buyCmd.stock)
                Assert.AreEqual(10.0, buyCmd.buy)
                Assert.AreEqual(2023, buyCmd.year)
                Assert.AreEqual("tsla", sellCmd.stock)
                Assert.AreEqual(5.0, sellCmd.sell)
                Assert.AreEqual(2024, sellCmd.year)
                Assert.AreEqual(1000, addCapitalCmd.amount)
                Assert.AreEqual(2023, addCapitalCmd.year)
                // Success for Bargraph Output
                Assert.IsTrue(true) 
            | _ -> Assert.Fail("Parsed AST structure is incorrect.")
        | None -> Assert.Fail("Parser failed to parse the entire program.")



    [<TestMethod>]
    member this.EmptyInputTest () =
        let programInput = ""
        let result = parse programInput
        match result with
        | Some(ast) when ast = [] -> Assert.IsTrue(true)  // Expecting an empty AST
        | None -> Assert.IsTrue(true)  // Or expecting a None, indicating no input to parse
        | _ -> Assert.Fail("Parser did not handle empty input correctly.")



    [<TestMethod>]
    member this.IncorrectlyFormattedInputTest () =
        let programInput = "buy(gold,10,2023 sell(tsla,5,2024"  // Missing closing parenthesis
        let result = parse programInput
        match result with
        | None -> Assert.IsTrue(true)  // Expecting parsing to fail
        | Some(_) -> Assert.Fail("Parser incorrectly parsed a badly formatted input.")
