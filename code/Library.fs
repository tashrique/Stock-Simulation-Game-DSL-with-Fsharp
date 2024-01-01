module Library
open System
open System.Diagnostics
open System.IO


(*
    Prints the usage message and exits the program with exit code 1.
*)
let usage () = 
    printfn "Usage: start\n
    Commands to use:
    - To add money: addcapital(<howMuchMoneyYouWantToAddToYourInvestments>)
    - To buy stocks: buy(<stockName>, <howMuchToBuy>)
    - To sell stocks: sell(<stockName>, <howMuchToSell>
    - To see your portfolio: output(portfolio)
    - To see a bargraph of your portfolio: output(bargraph)
    - To see a timeseries of your portfolio: output(timeseries)

    Available stocks: GOLD, SLVR, TSLA
    To go to the next year: next
    To exit the game: exit

    Things to keep in mind: 
    - You can't sell more than you have of a stock
    - You can't buy more than you have money for
    - Don't forget to select what type of output you want to see at the end of the game!

    Try running again.
    "
    exit 1


(*
    Prints the start message for the game
*)
let printStartMessage () = 
    printfn "Welcome to the Stock Simuation Game:
    
    
    ██╗  ██╗ █████╗ ██╗      █████╗ ██╗          ██████╗  █████╗ ███╗   ███╗██████╗ ██╗     ███████╗
    ██║  ██║██╔══██╗██║     ██╔══██╗██║         ██╔════╝ ██╔══██╗████╗ ████║██╔══██╗██║     ██╔════╝
    ███████║███████║██║     ███████║██║         ██║  ███╗███████║██╔████╔██║██████╔╝██║     █████╗  
    ██╔══██║██╔══██║██║     ██╔══██║██║         ██║   ██║██╔══██║██║╚██╔╝██║██╔══██╗██║     ██╔══╝  
    ██║  ██║██║  ██║███████╗██║  ██║███████╗    ╚██████╔╝██║  ██║██║ ╚═╝ ██║██████╔╝███████╗███████╗
    ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝     ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚═════╝ ╚══════╝╚══════╝                                                                

    
    \n
    The rules are simple: You start with a certain cash and you can buy and sell stocks. 
    The goal is to make as much money as possible.
        
    Once you type \"start\", you will be taken to a pdf document with stock market data from 2010 to 2015. 
    Right now in time, you are in 2016 and you will be able to make trades based on the data you see on the document. 
    Once you are done, you can type \"next\" and you will be taken to another document with 2010-2016 stock data and you can make trades in 2017
    This continues until 2020 and at the end you can see the result of your trades! 

    Available stocks right now in market: GOLD, SLVR, TSLA
    Commands to use:
    - To add money: addcapital(<howMuchMoneyYouWantToAddToYourInvestments>)
    - To buy stocks: buy(<stockName>, <howMuchToBuy>)
    - To sell stocks: sell(<stockName>, <howMuchToSell>)

    - To see your portfolio: output(portfolio)
    - To see a bargraph of your portfolio: output(bargraph)
    - To see a timeseries of your portfolio: output(timeseries)

    Dont' forget to select what type of output you want to see at the end of the game!

    You can type \"exit\" at any time to exit the game.

    Type \"start\" to begin the game.\n"



(*
    Removes all whitespace characters from a given string.

    @input: The string from which to remove whitespace.
    @Returns: A new string with all whitespace characters removed.
*)
let removeWhitespace (input: string) : string =
    input 
    |> Seq.filter (fun c -> not (System.Char.IsWhiteSpace(c)))// || c = '\n' 
    |> Seq.toArray 
    |> String



(*
    Turns all characters in a given string to lowercase.

    @input: The string to turn lowercase.
    @Returns: A new string all lowercase.
*)
let toLower (input: string) : string =
    input.ToLower()


(*
    Formats the user input by removing all whitespace and turning all characters to lowercase.

    @input: The string to format.
    @Returns: A new string with all whitespace removed and all characters lowercase.
*)
let formatInput (input: string) : string =
    input |> removeWhitespace |> toLower


(*
    Takes multiline user input from the console 5 times and returns it as a string. If user types "exit" at any time, the function returns the input.
    If users types "next" the counter is increased by 1 and the function is called recursively.

    @return the user input in string format
*)
let takeUserInput () = 
    let rec takeUserInputHelper (input : string) (year: int) = 

        let userInput = Console.ReadLine()
        let formattedInput = formatInput userInput
        let newInput = input + formattedInput // + "\n"

        match userInput, year with
        | _, 2020 -> 
            printfn "You have reached the end of the game. Your result is being generated."
            input
        | "exit", _ -> 
            printfn "You have exited the game. Your result is being generated."
            input
        | "next", _ -> 
            printfn "You are now in %d" (year + 1)

            let filename = "\\data\\" + string(year) + ".pdf"

            // Check current directory and file existence
            let currentDir = Directory.GetCurrentDirectory()
            let file = currentDir + filename

            if File.Exists(file) then
                let psi = new ProcessStartInfo(file)
                psi.UseShellExecute <- true
                Process.Start(psi) |> ignore
            else
                printfn "File not found: %s" file
            
            takeUserInputHelper input (year + 1)
        | _, y ->
            let yearAddedInput = ( newInput[0..(newInput.Length - 2)] + "," + string(year) + ")" )
            takeUserInputHelper  yearAddedInput y

    (takeUserInputHelper "" 2016)



(*
    Starts the game

    @returns true if the user input is "start", false otherwise
*)
let startGame () = 
    let input = Console.ReadLine()
    if formatInput input = "start" then
        printfn "You have started the game. You are now in 2016."
        true
    else
        false



(*
    Displays game rules, reads the user input and returns it as a string.

    @return the user input in string format
*)
let startAndReadInput () = 
    printStartMessage ()
    let start = startGame ()
    if start = true then

        let filename = "\\Data\\2015.pdf"

        // Check current directory and file existence
        let currentDir = Directory.GetCurrentDirectory()
        let file = currentDir + filename

        if File.Exists(file) then
            let psi = new ProcessStartInfo(file)
            psi.UseShellExecute <- true
            Process.Start(psi) |> ignore
        else
            printfn "File not found: %s" file


        let input = takeUserInput ()
        formatInput input
    else
        usage ()