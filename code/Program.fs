open Parser
open Evaluator
open Library
open Math


(* 
* The main function of the application. It reads input, parses it into an AST, 
* evaluates the AST for transactions, performs calculations, displays ouyput and returns an exit status.

* @param argv: Command line arguments.
* @return: Exit status code (0 for success, 1 for parsing failure).
*)
[<EntryPoint>]
let main argv =
    let input = startAndReadInput ()
    let ast = parse input
    match ast with
    | Some ast ->
        let userTransactions = evaluate ast
        calculate userTransactions
        0
    | None -> 
        printfn "Invalid Stock Transations, please try again."
        usage ()
        1


(*
Things to ask dan
- semantics specification (not sre what to put on it)
- test suite??
- checklist number k (does the welcome and the usage message suffice?)
- checklist o, running on project level / solution level?
*)


(*
TO IMPLEMENT notes

- Adjust for inflation
- Allow initialcapital only one time [done]
- Make sure users dont sell more than they buy [done]
- what if i never sell [done]
- give option to save my pdf portfolio and graphs [done]
*)