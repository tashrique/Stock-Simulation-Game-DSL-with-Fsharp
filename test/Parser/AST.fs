module AST


(*
   BNF Grammar

    <stock> ::= GOLD | SLVR | TSLA
    <transactionAmount> ::= <d><transactionAmount> | <d>
    <d> ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9

    <command> ::= buy(<stock>, <transactionAmount>, <year>) | sell(<stock>, <transactionAmount>, <year>) | addcapital(<transactionAmount>)
    
    <line> ::= <command> | <output> 
    <program> ::= <line> | <line><program>

    <output> ::= output(<outputtype>)
    <graph> ::= bargraph | timeseries | portfolio
*)

type Buy = {stock: string; buy: float; year: int}
type Sell = {stock: string; sell: float; year: int}
type AddCapital = {initial: string; amount: int; year: int}

type Command = 
    | BuyCommand of Buy
    | SellCommand of Sell
    | AddCapitalCommand of AddCapital
    
type Bargraph = string
type Timeseries = string
type Portfoio = string

type Output = 
    |Bargraph 
    |Timeseries 
    |Portfolio

type Line = Command of Command | Output of Output
type Program = Line list