module Parser
open AST
open Combinator

(*
* Parses integers, handling both negative and positive numbers.
* @return Parsed integer.
*)
let numberParser = 
    (pmany1 pdigit |>> (fun digits -> stringify digits |> int))

(*
* Parses keywords representing stock types: gold, slvr, or tsla.
* @return Parsed stock
*)
let GOLDparser = pstr "gold" |>> (fun x -> x)
let SLVRParser = pstr "slvr" |>> (fun x -> x)
let TSLAParser = pstr "tsla" |>> (fun x -> x)

(*
* Parses a stock type
* @return Parsed stock
*)
let stockParser: Parser<string> = 
    (GOLDparser <|> SLVRParser <|> TSLAParser) 
    |>> (fun x -> x |> string)
    

(*
* Parses buy command
* @return BuyCommand AST node with stock, amount, and year
*)
let buyParser : Parser<Command> =
    pseq 
        (pright (pstr "buy(") stockParser) 
        (pseq 
            (pbetween (pchar ',') numberParser (pchar ','))
            (pleft numberParser (pchar ')'))
            (fun (amount, year) -> (amount, year)))
        (fun (stock, (amount, year)) -> BuyCommand({ stock = stock; buy = amount; year = year }))

(*
* Parses sell command
* @return SellCommand AST node with stock, amount, and year
*)
let sellParser : Parser<Command> =
    pseq 
        (pright (pstr "sell(") stockParser) 
        (pseq 
            (pbetween (pchar ',') numberParser (pchar ','))
            (pleft numberParser (pchar ')'))
            (fun (amount, year) -> (amount, year)))
        (fun (stock, (amount, year)) -> SellCommand({ stock = stock; sell = amount; year = year }))


(*
* Parses addcapital command
* @return AddCapitalCommand AST node with amount and year
*)
let AddCapitalParser : Parser<Command> =
    let amountAndYearParser = 
        pseq
            (pbetween
                (pstr "addcapital(") 
                (numberParser)
                (pchar ','))
            (pleft numberParser (pchar ')'))
    amountAndYearParser (fun (amount, year) -> AddCapitalCommand({initial = "INITIAL"; amount = amount; year = year}))

(*
* Parses command
* @return Command AST node corresponding to the parsed command
*)
let commandParser: Parser<Line> = 
    buyParser <|> sellParser <|> AddCapitalParser
    |>> (fun x -> Command(x))


(*
* Parses bargraph output
* @return Bargraph AST node
*)
let bargraphParser = 
    pleft
        (pstr "bargraph,")
        numberParser
        |>> (fun _ -> Bargraph)


(*
* Parses timeseries output
* @return Timeseries AST node
*)
let timeseriesParser = 
    pleft
        (pstr "timeseries,")
        numberParser
        |>> (fun _ -> Timeseries)

(*
* Parses portfolio output
* @return Portfolio AST node
*)
let portfolioParser = 
    pleft
        (pstr "portfolio,")
        numberParser
        |>> (fun _ -> Portfolio)

(*
* Parses graph
* @return Output AST node corresponding to the parsed graph
*)
let graphParser: Parser<Output> = bargraphParser <|> timeseriesParser <|> portfolioParser

(* 
* Parses an output command
* @return: Output AST node corresponding to the parsed output
*)
let outputParser: Parser<Line> = 
    pbetween 
        (pstr "output(") (graphParser) (pchar ')') 
    |>> (fun output -> Output(output))

(* 
* Parses a line of input
* @return: Line AST node corresponding to the parsed input
*)
let lineParser: Parser<Line> = commandParser <|> outputParser

(*
* Parses the entire program
* @return: Program AST node representing the parsed program
*)
let programParser: Parser<Program> = 
    let singleLineParser = lineParser |>> (fun t -> [t])
    let multiLineParser =
        pseq
            singleLineParser
            (pmany0 singleLineParser)
            (fun (l,ls) -> l @ List.concat ls) 
    multiLineParser

let grammar = pleft programParser peof

(*
    Parses a string and returns an AST if the string is valid. Otherwise, returns None.

    @input: The string to parse.
    @returns: An AST if the string is valid. Otherwise, None.
*)
let parse (input: string) : Program option =
    let i = prepare input
    match grammar i with
    | Success(ast, _) -> 
        Some ast
    | Failure(pos,rule) ->
        printfn "Invalid expression."
        let msg = sprintf "Cannot parse input at position %d in rule '%s':" pos rule
        let diag = diagnosticMessage 20 pos input msg
        printf "%s" diag
        None


(*
    Attempts to parse a given string input into an Abstract Syntax Tree (AST) and prints it.
    If parsing is successful, the AST is displayed and the function returns 0.
    If parsing fails, indicating invalid input, an error message is printed and the function returns 1.

    @param input: The string to be parsed into an AST.
    @return: An integer status code - 0 for successful parsing and AST display, 1 for failure.
*)
let printAST (input : string) = 
    match parse input with
    | Some ast ->
        printfn "%A" ast
        0 
    | None -> 
        printfn "Invalid Stock Transations, please try again."
        1