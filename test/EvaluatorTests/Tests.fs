namespace EvaluatorTests

open AST
open Evaluator
open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestMethodPassing () =
        Assert.IsTrue(true);


    [<TestMethod>]
    member this.EvalBuyTest () =
        transactions.Clear()
        capital[0] <- 0
        evalBuy {stock = "gold"; buy = 10.0; year = 2023}
        Assert.AreEqual(10.0, transactions["GOLD2023B"])


    [<TestMethod>]
    member this.EvalSellTest () =
        transactions.Clear()
        capital[0] <- 0

        evalSell {stock = "tsla"; sell = 5.0; year = 2024}
        Assert.AreEqual(5.0, transactions["TSLA2024S"])


    [<TestMethod>]
    member this.EvalAddCapitalTest () =
        transactions.Clear()
        capital[0] <- 0

        evalAddCapital {initial= "INITIAL"; amount = 1000; year = 2018}
        Assert.AreEqual(1000, capital.[0])


    [<TestMethod>]
    member this.EvalCommandTest () =
        transactions.Clear()
        capital[0] <- 0

        // Test evalCommand with BuyCommand
        evalCommand (BuyCommand {stock = "gold"; buy = 10.0; year = 2023})
        Assert.AreEqual(10.0, transactions["GOLD2023B"])

        // Test evalCommand with SellCommand
        evalCommand (SellCommand {stock = "tsla"; sell = 5.0; year = 2024})
        Assert.AreEqual(5.0, transactions["TSLA2024S"])

        // Test evalCommand with AddCapitalCommand
        evalCommand (AddCapitalCommand {initial= "INITIAL"; amount = 1000; year = 2018})
        Assert.AreEqual(1000, capital[0])


    [<TestMethod>]
    member this.EvalOutputTest () =
        transactions.Clear()
        capital[0] <- 0

        // Test evalOutput with Bargraph
        evalOutput Bargraph
        Assert.AreEqual(1.0, transactions["bargraph"])

        // Test evalOutput with Timeseries
        evalOutput Timeseries
        Assert.AreEqual(1.0, transactions["timeseries"])

        // Test evalOutput with Portfolio
        evalOutput Portfolio
        Assert.AreEqual(1.0, transactions["portfolio"])


    [<TestMethod>]
    member this.EvalProgramTest () =
        transactions.Clear()
        capital[0] <- 0

        // Test evalProgram with a mix of commands
        let program = [Command (BuyCommand {stock = "gold"; buy = 10.0; year = 2023});
                    Command (SellCommand {stock = "tsla"; sell = 5.0; year = 2024});
                    Command (AddCapitalCommand {initial= "INITIAL"; amount = 1000; year = 2014})]
        evalProgram program
        Assert.AreEqual(10.0, transactions["GOLD2023B"])
        Assert.AreEqual(5.0, transactions["TSLA2024S"])
        Assert.AreEqual(1000, capital[0])




    [<TestMethod>]
    member this.EvaluateTest () =
        transactions.Clear()
        capital[0] <- 0
        let program = [Command (BuyCommand {stock = "gold"; buy = 10.0; year = 2023});
                    Command (SellCommand {stock = "tsla"; sell = 5.0; year = 2024});
                    Output Bargraph]
        let result = evaluate program
        Assert.IsTrue(result.ContainsKey("GOLD2023B") && result.ContainsKey("TSLA2024S") && result.ContainsKey("bargraph"))



    [<TestMethod>]
    member this.NegativeValuesTest () =
        transactions.Clear()
        capital[0] <- 0

        // Test negative values
        evalBuy {stock = "gold"; buy = -10.0; year = 2023}
        evalSell {stock = "tsla"; sell = -5.0; year = 2024}
        evalAddCapital {initial= "INITIAL"; amount = -1000; year = 2014}

        Assert.IsTrue(transactions["GOLD2023B"] < 0.0)
        Assert.IsTrue(transactions["TSLA2024S"] < 0.0)
        Assert.IsTrue(capital[0] < 0)


    [<TestMethod>]
    member this.DuplicatedTransactionsTest () =
        transactions.Clear()
        capital[0] <- 0

        // Test duplicated transactions
        evalBuy {stock = "gold"; buy = 10.0; year = 2023}
        evalBuy {stock = "gold"; buy = 15.0; year = 2023}

        Assert.AreEqual(25.0, transactions.["GOLD2023B"])
