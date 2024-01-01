'''
This python file will take in the parsed data from the parser and construct the result through the evaluator.
It is uploaded in the git repo for the purpose of testing the parser and evaluator and to show progress.
'''



import matplotlib.pyplot as plt
from matplotlib import style

style.use('ggplot')

#------------------STATIC VARIBALES------------------#


# Initial transactions all set to zero. +number means you sold, -number means you bought. At the end, it will be adjusted with the profits/losses.
# "GOLD": [sum, 2016, 2017, 2018, 2019, 2020]
transactions = {
    "GOLD": [0, 0, 0, 0, 0, 0], 
    "SLVR": [0, 0, 0, 0, 0, 0],
    "TSLA": [0, 0, 0, 0, 0, 0],
}

portfolio_value = {}
profit_loss = {}


'''
Varaibles to be mutated by parsing
'''
initial = 0
program = {}
#What sort of output the user wants (bargraph, portfolio, timeseries)
output = set()






#------------------INPUT VARIABLES FROM PARSING------------------#
initial = 1000
'''
GOLD2016B = bought 40 dollars woth of gold in 2016
[B = bought, S = sold]
'''
program = {
    "GOLD2016B": 40, #bought 40 dollars woth of gold in 2016 
    "GOLD2018S": 40, #sold 40 dollars worth of gold in 2018
    "SLVR2016B": 100,
    "SLVR2017B": 100,
    "SLVR2020S": 150,
    "TSLA2016B": 100,
    "TSLA2017B": 100,
    "TSLA2018B": 100,
    "TSLA2019B": 100,
    "TSLA2020S": 400,
    "portfolio": True,
    "bargraph": True,
    "timeseries": True
}
'''REMEMBER TASHHHH ->>>>  F# dicts is not the same notation as python dictionary. Might need string manipulation to get the key and value! ooorrrr
maybe just use the evaluator to manipulate the string as python readable'''

#------------------ALL THE MATH------------------#


# Initialize a dictionary to store total value per year
portfolio_value = {str(year): initial for year in range(2015, 2021)}

profit_loss = {
    "2015": 0, 
    "2016": 0, 
    "2017": 0, 
    "2018": 0, 
    "2019": 0, 
    "2020": 0
}

'''
a years portfolio volume = initial - 
'''
# Calculate the value of the portfolio for each year
for line, amount in program.items():
    
    #Update the output set
    if amount == True:
        output.add(line)
    else:
        stock = line[0:4]
        year = line[4:8]
        transactiontype = line[8]

        if stock not in transactions:
            transactions[stock] = [0, 0, 0, 0, 0, 0]
            portfolio_value[year] = 0
            profit_loss[year] = 0


        # Apply the transaction to the initial amount
        if transactiontype == "B":
            transactions[stock][int(year) - 2015] -= amount
            transactions[stock][0] -= amount


        elif transactiontype == "S":
            transactions[stock][int(year) - 2015] += amount
            transactions[stock][0] += amount

        else:
            print("ERROR: Transaction type not found")



# Calculate the value of the portfolio for each year
for stock, transaction in transactions.items():
    for i in range(1, len(transaction)):
        year = str(i + 2015)
        portfolio_value[year] += transaction[i]




# Now you can plot the portfolio value over time
years = sorted(portfolio_value.keys())
values = [portfolio_value[year] for year in years]






#------------------OUTPUT GENERATING FUNCTIONS------------------#

def generate_portfolio():
    #Some sort of pdf creating function
    pass


'''
I need:


THIS ONE IS DEMO FUNCTION
'''
def generate_bar_graph():
    plt.bar(years, values, color="orange") #plot total investment+profit/loss with time
    plt.xlabel("Years")
    plt.ylabel("Total Investment ($)")
    plt.title("Bar Graph (Individual Investment ($) vs )")
    plt.grid(True)
    pass



'''
I need:
- year (x-axis)
- total value in that particular year (y-axis)
'''
def generate_time_series():
    plt.plot(list(portfolio_value.keys()), list(portfolio_value.values()), marker='o') #plot total investment+profit/loss with time
    plt.xlabel("Years")
    plt.ylabel("Total Investment ($)")
    plt.title("Time Series (Total Investment ($) vs Years)")
    plt.grid(True)


def generate_output():
    if "portfolio" in output:
        generate_portfolio()

    if "bargraph" in output and "timeseries" in output:        
        plt.figure(figsize=(10, 5)) # (width, height)
        plt.tight_layout()

        plt.subplot(1, 2, 1)  # (rows, columns, subplot number)
        generate_bar_graph()
        plt.subplot(1, 2, 2)
        generate_time_series()

    elif "timeseries" in output and "bargraph" not in output:
        generate_time_series()
    elif "bargraph" in output and "timeseries" not in output:
        generate_bar_graph()
    plt.show()

    



#------------------MAIN------------------#
# print(output)
print(transactions)
# print(profit_loss)
print(portfolio_value)
generate_output()