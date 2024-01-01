module Math
open System.Collections.Generic
open Library

(*
* Relative rates of return for gold, silver and tesla stocks from 2015 to 2020
*)
let rates = dict [
    "GOLD20152016", 1.09
    "GOLD20152017", 1.22
    "GOLD20152018", 1.2
    "GOLD20152019", 1.43
    "GOLD20152020", 1.79

    "GOLD20162017", 1.13
    "GOLD20162018", 1.11
    "GOLD20162019", 1.31
    "GOLD20162020", 1.64

    "GOLD20172018", 0.98
    "GOLD20172019", 1.2
    "GOLD20172020", 1.45

    "GOLD20182019", 1.18
    "GOLD20182020", 1.47

    "GOLD20192020", 0.8


    "SLVR20152016", 1.15
    "SLVR20152017", 1.22
    "SLVR20152018", 1.12
    "SLVR20152019", 1.29
    "SLVR20152020", 1.91

    "SLVR20162017", 1.06
    "SLVR20162018", 0.97
    "SLVR20162019", 1.12
    "SLVR20162020", 1.66

    "SLVR20172018", 0.91
    "SLVR20172019", 1.05
    "SLVR20172020", 1.56

    "SLVR20182019", 1.15
    "SLVR20182020", 1.71

    "SLVR20192020", 1.49


    "TSLA20152016", 0.89
    "TSLA20152017", 1.298
    "TSLA20152018", 1.38
    "TSLA20152019", 1.743
    "TSLA20152020", 11.761

    "TSLA20162017", 1.46
    "TSLA20162018", 1.56
    "TSLA20162019", 1.97
    "TSLA20162020", 13.21

    "TSLA20172018", 1.06
    "TSLA20172019", 1.34
    "TSLA20172020", 8.99

    "TSLA20182019", 1.27
    "TSLA20182020", 8.53

    "TSLA20192020", 6.72
]
let years = ["2016"; "2017"; "2018"; "2019"; "2020"]


let transactions = Evaluator.transactions |> Seq.map (fun kvp -> (kvp.Key, float(kvp.Value))) |> Seq.toList
let initial = float(Evaluator.capital[0])
let currentCapital = Array.create 1 initial

let mutable output = []
let yearlyCapital = Array.create 6 0.0
yearlyCapital[0] <- initial
let yearlybuySell = Array.create 6 0.0
let totalYearlyProfit = Array.create 6 0.0
let portfolioValueWithProfit = Array.create 6 0.0
portfolioValueWithProfit[0] <- initial


let stocksIndividual : IDictionary<string, float[]> = dict [
    "GOLDB", Array.create 5 0.0
    "GOLDS", Array.create 5 0.0
    "TSLAB", Array.create 5 0.0
    "TSLAS", Array.create 5 0.0
    "SLVRB", Array.create 5 0.0
    "SLVRS", Array.create 5 0.0
]
let profitIndividual = dict [
    "GOLD", Array.create 1 0.0
    "TSLA", Array.create 1 0.0
    "SLVR", Array.create 1 0.0

]
let stocks = ["GOLD"; "TSLA"; "SLVR"]
let starting = Array.create 3 0.0
let ending = Array.create 3 0.0


(*
    Processes a list of transactions to calculate the net buy or sell amount for each year.
    @param transactions: List of transactions (key-value pairs).
    @return: Array of net buy/sell amounts for each year.
*)
let rec calculateYearlyBuySell transactions =
    match transactions with
    | [] -> yearlybuySell
    | (k, v)::ts ->
        if k = "portfolio" || k = "bargraph" || k = "timeseries" then
            calculateYearlyBuySell ts
        else
            let year = k.Substring(k.Length - 5, 4)
            let ttype = k.Substring(k.Length - 1, 1)

            if ttype = "B" then
                yearlybuySell[int(year) - 2015] <- yearlybuySell[int(year) - 2015] - v
                calculateYearlyBuySell ts
                
            elif ttype = "S" then
                yearlybuySell[int(year) - 2015] <- yearlybuySell[int(year) - 2015] + v
                calculateYearlyBuySell ts
            else
                calculateYearlyBuySell ts


(*
    Processes transactions to calculate the starting and ending values of each stock.
    @param transactions: List of stock transactions.
    @return: Tuple of arrays containing starting and ending values for each stock.
*)
let rec startsEnds (transactions: list<string * float>) =
    match transactions with
    | [] -> starting, ending
    | (k, v)::ts ->
        if k = "portfolio" || k = "bargraph" || k = "timeseries" then
            startsEnds ts
        else
            let stock = k.Substring(0, k.Length - 5)
            let year = k.Substring(k.Length - 5, 4)
            let index = stocks |> Seq.findIndex (fun x -> x = stock)
            let ttype = k.Substring(k.Length - 1, 1)

            if ttype = "B" then
                starting[index] <- starting[index] + v
                startsEnds ts
                
            elif ttype = "S" then

                ending[index] <- ending[index] + v
                if ending[index] > starting[index] then
                    printfn "Nah dude, you cannot sell more than you buy for %s. Basic maths yo!\n" stock
                    printfn "You bought $%.2f and sold $%.2f in %s\nStart again." starting[index] ending[index] year
                    usage ()
                startsEnds ts
            else
                startsEnds ts


(*
    Calculates the yearly capital based on the net buy/sell amounts for each year.
    @param yearlybuySell: List of yearly net buy/sell amounts.
    @return: Array of yearly capital values.
*)
let calculateYearlyCapital (yearlybuySell: float list) =
    let rec helper i =
        match i with
        | 5 -> yearlyCapital
        | i ->
            yearlyCapital[i + 1] <- yearlyCapital[i] + yearlybuySell[i + 1]
            helper (i + 1)
    helper 0


(*
    Extracts the types of outputs (like portfolio, bargraph, timeseries) from the transactions.
    @param transactions: List of transactions.
    @return: List of output types.
*)
let rec getOutputTypes (transactions: list<string * float>) =
    match transactions with
    | [] -> output
    | (k, s)::ts ->
        if k = "portfolio" || k = "bargraph" || k = "timeseries" then
            output <- output @ [k]
            getOutputTypes ts
        else
            getOutputTypes ts


(*
   Calculates the quantity of each stock bought or sold each year.
    @param transactions: List of transactions.
    @return: Dictionary with stock types as keys and arrays of quantities for each year.
*)
let rec calculateStocksIndividual (transactions: list<string * float>) =
    match transactions with
    | [] -> stocksIndividual
    | (k, v)::ts ->
        if k = "portfolio" || k = "bargraph" || k = "timeseries" then
            calculateStocksIndividual ts
        else
            let stock = k.Substring(0, k.Length - 5)
            let year = k.Substring(k.Length - 5, 4)
            let ttype = k.Substring(k.Length - 1, 1)

            let key = stock + ttype
            stocksIndividual[key][int(year) - 2016] <- float(stocksIndividual[key][int(year) - 2016]) + float(v)
            calculateStocksIndividual ts

(*
    Calculates the portfolio value with profit for each year.
    @param years: List of years.
    @return: Array of portfolio values with profit for each year.
*)
let rec calculatePortfolioValueWithProfit years =
    match years with
    | [] -> portfolioValueWithProfit
    | y::ys ->
        let index = int(y) - 2015
        portfolioValueWithProfit[index] <- yearlyCapital[index] + totalYearlyProfit[index]
        calculatePortfolioValueWithProfit ys



(*
    Calculates the total yearly profit from stock transactions.
    @param years: List of years.
    @return: Array of total yearly profits.
*)
let rec calculateTotalYearlyProfit years =
    match years with
    | [] -> totalYearlyProfit
    | y::ys ->
        let index = int(y) - 2016

        let gold = stocksIndividual["GOLDS"][index]
        let slvr = stocksIndividual["SLVRS"][index]
        let tsla = stocksIndividual["TSLAS"][index]

        let goldRate = rates["GOLD" + (string(int(y) - 1)) + y]
        let slvrRate = rates["SLVR" + (string(int(y) - 1)) + y]
        let tslaRate = rates["TSLA" + (string(int(y) - 1)) + y]

        let goldProfit = float(gold) * goldRate
        let slvrProfit = float(slvr) * slvrRate
        let tslaProfit = float(tsla) * tslaRate

        profitIndividual["GOLD"][0] <- profitIndividual["GOLD"][0] + goldProfit
        profitIndividual["SLVR"][0] <- profitIndividual["SLVR"][0] + slvrProfit
        profitIndividual["TSLA"][0] <- profitIndividual["TSLA"][0] + tslaProfit
        totalYearlyProfit[index + 1] <- goldProfit + slvrProfit + tslaProfit

        calculateTotalYearlyProfit ys



(*
    Calculates the current capital after each transaction, ensuring that purchases don't exceed available capital.
    @param transactions: List of transactions.
    @return: Array with the current capital after processing all transactions.
*)
let rec calculateCurrentCapital (transactions: list<string * float>) =
    match transactions with
    | [] -> currentCapital
    | (k, v)::ts ->
        if k = "portfolio" || k = "bargraph" || k = "timeseries" then
            calculateCurrentCapital ts
        else
            let ttype = k.Substring(k.Length - 1, 1)
            if ttype = "B" then
                if currentCapital[0] - v < 0.0 then
                    printfn "Dude, you cannot buy more than you have. You have $%.2f left" currentCapital[0]
                    usage ()
                currentCapital[0] <- currentCapital[0] - v
                calculateCurrentCapital ts
            elif ttype = "S" then
                currentCapital[0] <- currentCapital[0] + v
                calculateCurrentCapital ts
            else
                calculateCurrentCapital ts




(*
    Main function to calculate necessary financial figures and visualize the data.
    Processes the transactions, calculates buy/sell amounts, yearly capital, stock quantities, and profits. 
    Finally, calls the visualization function with calculated data.
    @param input: Dictionary of transactions.
*)
let calculate (input: Dictionary<string,float>) =
    startsEnds transactions |> ignore
    calculateCurrentCapital transactions |> ignore
    let buysells = calculateYearlyBuySell transactions
    let buySellTemp = buysells |> Array.toList
    calculateYearlyCapital buySellTemp |> ignore
    getOutputTypes transactions |> ignore
    calculateStocksIndividual transactions |> ignore
    calculateTotalYearlyProfit years |> ignore
    calculatePortfolioValueWithProfit years |> ignore

    Chart.visualize 
        output 
        initial
        (totalYearlyProfit  |> Array.toList)
        (yearlyCapital |> Array.toList)
        (portfolioValueWithProfit |> Array.toList)
        stocks 
        (starting |> Array.toList)
        (ending |> Array.toList)
        (yearlybuySell |> Array.toList)
    |> ignore