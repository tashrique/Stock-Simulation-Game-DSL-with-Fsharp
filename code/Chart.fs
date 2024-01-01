module Chart
open Plotly.NET
open Aspose.Pdf
open Aspose.Pdf.Text
open System.Diagnostics


let years = ["2015"; "2016"; "2017"; "2018"; "2019"; "2020"]

(*
    Generates a time series chart based on yearly capital and portfolio value.
    This function creates two line charts: one for yearly capital changes and another for the total value of the portfolio over time.
    These charts are combined and titled to provide a visual representation of the portfolio's performance over the years 2015 to 2020.

    @param yearlyCapital: A list of floats representing yearly capital.
    @param portfolioValueWithProfit: A list of floats representing the portfolio value with profit for each year.
    @return: A Chart object representing the time series.
*)
let drawTimeseries (yearlyCapital: float list) (portfolioValueWithProfit: float list) =
    let timeseries = 
        [   Chart.Line(x = years, y = yearlyCapital, Name = "Yearly Capital Change")
            Chart.Line(x = years, y = portfolioValueWithProfit, Name = "Total Portfolio Value") ]
        |> Chart.combine
        |> Chart.withTitle "Time Series of Portfolio Value and Yearly Capital Change w/ Time (2015-2020)" 
        |> Chart.withXAxisStyle (TitleText = "Amount in $")
        |> Chart.withYAxisStyle (TitleText = "Time")
    timeseries


(*
    Generates a bar graph based on the starting and ending values of stocks.
    This function creates two sets of bar charts: one showing the initial amounts of stocks purchased and another showing their values with profit or loss at the end.
    These charts are combined and titled to illustrate the individual performance of each stock from 2015 to 2020.

    @param stocks: A list of strings representing the stock names.
    @param starts: A list of floats representing the initial values of stocks.
    @param ends: A list of floats representing the ending values of stocks (with profit/loss).
    @return: A Chart object representing the bar graph.
*)
let drawBargraph (stocks: string list) (starts: float list) (ends: float list) =
    let bargraph = 
        [
            Chart.Column (starts, stocks, Name="Purchased Stock");
            Chart.Column (ends, stocks, Name="Sold Stock")
        ]
        |> Chart.combine
        |> Chart.withTitle "Bar Graph of Individual Stock Transactions (2015-2020)"
        |> Chart.withXAxisStyle (TitleText = "Amount in $")
    bargraph


(*
    Creates a detailed portfolio statement in PDF format.
    This function generates a comprehensive portfolio statement, including a title, description of initial capital and net profit, breakdowns of yearly capital and profit, portfolio values, and individual stock transactions.
    The information is styled and added to a PDF document, which is then saved with a timestamped filename and opened for viewing.

    @param initial: The initial capital as a float.
    @param totalYearlyProfit: A list of floats representing total yearly profit.
    @param yearlyCapital: A list of floats representing yearly capital.
    @param portfolioValueWithProfit: A list of floats representing the portfolio value with profit for each year.
    @param stocks: A list of strings representing the stock names.
    @param starts: A list of floats representing the initial values of stocks.
    @param ends: A list of floats representing the ending values of stocks (with profit/loss).
*)
let drawPortfolio  (initial: float) (totalYearlyProfit: float list) (yearlyCapital: float list) (portfolioValueWithProfit: float list) (stocks: string list) (starts: float list) (ends: float list) =

    (* Create pdf *)
    let pdf_doc = new Document()
    let page = pdf_doc.Pages.Add()


    (* Build strings  *)
    let date = System.DateTime.Now
    let netProfit = totalYearlyProfit |> List.sum  
    let title = "Stock Portfolio and Statement as of " + string date + "\n\n"
    let description = 
        "Your Initial Capital: $" + string initial + "\n\n" +
        "Net Profit: $" + string netProfit + "\n\n"


    let breakdowns = 
        "Your Yearly Capital by Years: \n\n" +
        "2015: $" + string yearlyCapital[0] + "\n" +
        "2016: $" + string yearlyCapital[1] + "\n" +
        "2017: $" + string yearlyCapital[2] + "\n" +
        "2018: $" + string yearlyCapital[3] + "\n" +
        "2019: $" + string yearlyCapital[4] + "\n" +
        "2020: $" + string yearlyCapital[5] + "\n\n" +

        "Your Total Yearly Profit by Years: \n\n" +
        "2015: $" + string totalYearlyProfit[0] + "\n" +
        "2016: $" + string totalYearlyProfit[1] + "\n" +
        "2017: $" + string totalYearlyProfit[2] + "\n" +
        "2018: $" + string totalYearlyProfit[3] + "\n" +
        "2019: $" + string totalYearlyProfit[4] + "\n" +
        "2020: $" + string totalYearlyProfit[5] + "\n\n" +

        "Your Portfolio Value with Profit by Years: \n\n" +
        "2015: $" + string portfolioValueWithProfit[0] + "\n" +
        "2016: $" + string portfolioValueWithProfit[1] + "\n" +
        "2017: $" + string portfolioValueWithProfit[2] + "\n" +
        "2018: $" + string portfolioValueWithProfit[3] + "\n" +
        "2019: $" + string portfolioValueWithProfit[4] + "\n" +
        "2020: $" + string portfolioValueWithProfit[5] + "\n\n" +

        "Your Stock Transactions: \n\n" +
        "Stock: " + string stocks[0] + "\n" +
        "Bought Total: $" + string starts[0] + "\n" +
        "Sold Total: $" + string ends[0] + "\n\n" +
        "Stock: " + string stocks[1] + "\n" +
        "Bought Total: $" + string starts[1] + "\n" +
        "Sold Total: $" + string ends[1] + "\n\n" +
        "Stock: " + string stocks[2] + "\n" +
        "Bought Total: $" + string starts[2] + "\n" +
        "Sold Total: $" + string ends[2] + "\n\n"


    let footer = "Portfolio generated electronically by Halal Gambler at " + string date + "\n\n "

    let titleF = new TextFragment(title)
    let descriptionF = new TextFragment(description)
    let breakdownsF = new TextFragment(breakdowns)
    let footerF = new TextFragment(footer)

    (* Styles *)
    titleF.TextState.Font <- FontRepository.FindFont("TimesNewRoman")
    titleF.TextState.FontSize <- 15.0f
    titleF.TextState.FontStyle <- FontStyles.Bold
    titleF.TextState.Underline <- true

    descriptionF.TextState.Font <- FontRepository.FindFont("TimesNewRoman")
    descriptionF.TextState.FontSize <- 11.0f

    breakdownsF.TextState.Font <- FontRepository.FindFont("TimesNewRoman")
    breakdownsF.TextState.FontSize <- 11.0f

    footerF.TextState.Font <- FontRepository.FindFont("TimesNewRoman")
    footerF.TextState.FontSize <- 8.0f
    footerF.TextState.FontStyle <- FontStyles.Italic

    page.Paragraphs.Add((titleF))
    page.Paragraphs.Add((descriptionF))
    page.Paragraphs.Add((breakdownsF))
    page.Paragraphs.Add((footerF))

    
    (* Get current directory and navigate two levels up *)
    let currentDir = System.IO.Directory.GetCurrentDirectory()
    let parentDir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(currentDir).FullName).FullName

    (* Save and open pdf *)
    let timeNow = System.DateTime.Now
    let formattedTime = timeNow.ToString("HH-mm_dd-MM-yyyy tt")
    let filename = "Portfolio_Halal_" + formattedTime + ".pdf"
    let fullPath = System.IO.Path.Combine(parentDir, filename)
    pdf_doc.Save(fullPath)

    let psi = new ProcessStartInfo(fullPath)
    psi.UseShellExecute <- true
    Process.Start(psi) |> ignore



(*
    Visualizes the output based on the specified parameters.
    This function determines which type of visualization to generate (portfolio, timeseries, and/or bargraph) based on the user's choices.
    It calls the appropriate functions to draw the selected charts or generate the portfolio PDF, ensuring that the desired output is displayed or created.

    @param output: A list of strings indicating the types of output to generate.
    @param initial: The initial capital as a float.
    @param totalYearlyProfit: A list of floats representing total yearly profit.
    @param yearlyCapital: A list of floats representing yearly capital.
    @param portfolioValueWithProfit: A list of floats representing the portfolio value with profit for each year.
    @param stocks: A list of strings representing the stock names.
    @param starts: A list of floats representing the initial values of stocks.
    @param ends: A list of floats representing the ending values of stocks (with profit/loss).
    @param yearlybuySell: A list of floats representing yearly buy and sell amounts.
*)
let visualize (output: string list) (initial: float) (totalYearlyProfit: float list) (yearlyCapital: float list) (portfolioValueWithProfit: float list) (stocks: string list) (starts: float list) (ends: float list) (yearlybuySell: float list) =

    let timeNow = System.DateTime.Now
    let formattedTime = timeNow.ToString("HH-mm_dd-MM-yyyy tt")
    let filename = "outputGraphs_Halal_" + formattedTime + ".html"
    let currentDir = System.IO.Directory.GetCurrentDirectory()
    let parentDir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(currentDir).FullName).FullName
    let fullPath = System.IO.Path.Combine(parentDir, filename)

    if List.contains "portfolio" output then
        drawPortfolio initial totalYearlyProfit yearlyCapital portfolioValueWithProfit stocks starts ends
    

    if List.contains "timeseries" output && List.contains "bargraph" output then
        let combinedChart = [drawTimeseries yearlyCapital portfolioValueWithProfit; drawBargraph stocks starts ends] |> Chart.Grid(2, 1)
        combinedChart |> Chart.show
        combinedChart |> Chart.saveHtml fullPath

    elif List.contains "timeseries" output then
        let timeSeriesChart = drawTimeseries yearlyCapital portfolioValueWithProfit
        timeSeriesChart |> Chart.show 
        timeSeriesChart |> Chart.saveHtml fullPath

    elif List.contains "bargraph" output then
        let barGraphChart = drawBargraph stocks starts ends
        barGraphChart |> Chart.show
        barGraphChart |> Chart.saveHtml fullPath


    elif List.contains "portfolio" output then
        printfn "Portfolio generated successfully to the folder: %s" parentDir
        exit 0


    else
        printfn "No output type was selected.\n Type 'output(<graph>)' where graph is one of portfolio, timeseries, or bargraph.\n Please try again.\n"
        exit 0
    
    printfn "Output generated successfully to the folder: %s" parentDir