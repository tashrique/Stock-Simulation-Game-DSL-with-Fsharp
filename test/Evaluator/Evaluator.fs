module Evaluator
open AST
open System.Collections.Generic

(* Stores all processed transactions and capital information. *)
let transactions = new Dictionary<string, float>()
let capital = Array.create 1 0

(* 
* Evaluates a Buy command and updates the transactions dictionary.
* @param buy: The Buy record containing the stock, amount, and year
*)
let evalBuy (buy: Buy) =
    let key = (buy.stock.ToUpper() + (buy.year.ToString()) + "B")
    if transactions.ContainsKey(key) then
        transactions.Item(key) <- transactions.Item(key) + buy.buy
    else
        transactions.Add(key, buy.buy)

(* 
* Evaluates a Sell command and updates the transactions dictionary.
* @param sell: The Sell record containing the stock, amount, and year. 
*)
let evalSell (sell: Sell) = 
    let key = (sell.stock.ToUpper() + (sell.year.ToString()) + "S")
    if transactions.ContainsKey(key) then
        transactions.Item(key) <- transactions.Item(key) + sell.sell
    else
    transactions.Add(key, sell.sell)

(* 
* Evaluates an AddCapital command and updates the capital array.
* @param initial: The AddCapital record containing the amount to be added. 
*)
let evalAddCapital (initial: AddCapital) =
        capital[0] <- capital[0] + initial.amount


(* 
* Evaluates a Command and delegates to the specific command evaluation functions.
* @param command: The Command variant to be evaluated. 
*)
let evalCommand (command: Command) =
    match command with
    | BuyCommand buy -> evalBuy buy
    | SellCommand sell -> evalSell sell
    | AddCapitalCommand capital -> evalAddCapital capital


(* 
* Evaluates an Output command and updates the transactions dictionary.
* @param output: The Output variant to be evaluated. 
*)
let evalOutput (output: Output) = 
    match output with
    | Bargraph -> 
        if transactions.ContainsKey("bargraph") then
            transactions.Item("bargraph") <- transactions.Item("bargraph") + 1.0
        else
            transactions.Add("bargraph", 1)
    | Timeseries -> 
        if transactions.ContainsKey("timeseries") then
            transactions.Item("timeseries") <- transactions.Item("timeseries") + 1.0
        else
            transactions.Add("timeseries", 1)
    | Portfolio -> 
        if transactions.ContainsKey("portfolio") then
            transactions.Item("portfolio") <- transactions.Item("portfolio") + 1.0
        else
            transactions.Add("portfolio", 1)


(* 
* Evaluates a single line of the program, which can be either a Command or an Output.
* @param line: The Line variant to be evaluated. 
*)
let evalLine (line: Line) = 
    match line with
    | Command command -> evalCommand command
    | Output output -> evalOutput output


(* 
* Recursively evaluates each line in the program.
* @param program: The list of Line variants representing the program. 
*)
let rec evalProgram (program: Program) =
    match program with
    | [] -> ()
    | l::ls -> evalLine l; evalProgram ls

(* 
* Evaluates the entire program and returns the updated transactions dictionary.
* @param program: The Program to be evaluated.
* @return: The updated transactions dictionary after evaluating the program. 
*)
let evaluate (program: Program) =
    evalProgram program |> ignore
    transactions