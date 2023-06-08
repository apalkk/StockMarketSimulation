using System;
using System.Xml.Linq;
using System.Linq;
using StockMarketSim;
using System.Collections.Generic;

public class Program
{
    public static List<StockMarketSim.Stock> Stock_List = new List<StockMarketSim.Stock>
        {
            new StockMarketSim.Stock("Sapphire Designs",0,1),
            new StockMarketSim.Stock("QuantumQube Technologies",1000000,2),
            new StockMarketSim.Stock("EpicQuest Adventures",33812.12,3),
            new StockMarketSim.Stock("GlobalGrowth Financial Services",712123.23,4),
            new StockMarketSim.Stock("Whisk & Roll Culinary School",45671.23,5),
            new StockMarketSim.Stock("Harmony Music Academy",150.23,6),
            new StockMarketSim.Stock("Blissful Bites Bakery",1221.31,7)
        };

    public static Dictionary<Stock, Double> Holdings = new Dictionary<Stock, Double>();
    public static double Money = 100000;

    public static Task Main(String[] args)
    {

        foreach(Stock stock in Stock_List)
        {
            Holdings.Add(stock,0);
            StockMarket.AddStock(stock);
        }

        Task.Run(StockThreads); Task.Run(StockThreads); Task.Run(StockThreads);


        Console.WriteLine("Commands:");
        Console.WriteLine("add $1 $2: Adds a stock of name $1 with stocks worth $2");
        Console.WriteLine("sell $1 $2: Sells $2 stocks of id $1");
        Console.WriteLine("buy $1 $2: Buys $2 stocks of id $1");
        Console.WriteLine("high: Prints the most expensive stock");
        Console.WriteLine("low: Prints the least expensive stock");
        Console.WriteLine("all: Prints all the stocks availaible");
        Console.WriteLine("name: Prints stocks sorted by name");
        Console.WriteLine("trans_a $1: Prints all transactions above $1 in descending order");
        Console.WriteLine("trans_b $1: Prints all transactions below $1 ind escending order");
        Console.WriteLine("sort $1: Prints all transactions of $1");
        Console.WriteLine("trans : Prints all transactions of all stocks");
        Console.WriteLine("holdings : Prints all owned stocks");
        Console.WriteLine("price : Prints all owned stocks");

        while (true)
        {
            try
            {
                string Console_Input = Console.ReadLine();
                string[] Console_Args = Console_Input.Split(' ');
                string Keyword = Console_Args[0];
    
                switch (Keyword)
                {
                    case "sell":
                        ConsoleSell(Console_Args[1], Console_Args[2]);
                        break;
                    case "buy":
                        ConsoleBuy(Console_Args[1], Console_Args[2]);
                        break;
                    case "add":
                        AddStockToMarket(Console_Args[1], Console_Args[2]);
                        break;
                    case "high":
                        PrintHighestStock();
                        break;
                    case "all":
                        PrintAllStocks();
                        break;
                    case "low":
                        PrintLowestStock();
                        break;
                    case "name":
                        PrintStocksSortedByName();
                        break;
                    case "trans_a":
                        try
                        {
                            PrintTransAbove(double.Parse(Console_Args[1]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Please input a valid integer");
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "trans_b":
                        PrintTransBelow(double.Parse(Console_Args[1]));
                        break;
                    case "sort":
                        PrintTransById(Console_Args[1]);
                        break;
                    case "trans":
                        PrintTrans();
                        break;
                    case "time":
                        PrintTime(Console_Args[1]);
                        break;
                    case "holdings":
                        PrintHoldings();
                        break;
                    case "money":
                        PrintMoneyLeft();
                        break;
                    default:
                        Console.WriteLine("Please input a valid command");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Incorrect Arguements");
                Console.WriteLine(e.Message);
            }
        }
    }

    private static void PrintMoneyLeft()
    {
        Console.WriteLine(Money);
    }

    private static void PrintHoldings()
    {
        foreach (var v in Holdings)
        {
            Console.WriteLine("Stock = {0}, Stocks Owned = {1}", v.Key.Name, v.Value);
        }

    }

    private static void PrintTime(string time_In_String)
    {
        var time = TimeSpan.Parse(time_In_String);
        var query = StockMarket.GetStocks().Where(s => s.Time_Updated.CompareTo(time) > 0).OrderByDescending(s => s.Time_Updated);
        foreach (var x in query)
        {
            Console.WriteLine(x.PrintStock());
        }
    }

    private static void ConsoleBuy(string id_In_String, string quantity_In_String)
    {
        try
        {
            var transaction = new StockMarketSim.Transaction(int.Parse(id_In_String), StockMarketSim.TransactionType.Buy, Double.Parse(quantity_In_String));

            if(transaction.Stock_Used.Curr_Price*transaction.Quantity > Money) { return; }
            Money -= transaction.Stock_Used.Curr_Price * transaction.Quantity;
            Holdings[transaction.Stock_Used] += transaction.Quantity;

            transaction.Execute();
            lock (StockMarket.GetTransactions())
            {
                StockMarket.AddTransaction(transaction);
            }
        }
        catch (FormatException e)
        {
            Console.WriteLine("Please input a valid id/quantity number");
            Console.WriteLine(e.Message);
        }
    }

    private static void ConsoleSell(string id, string quantity)
    {
        try
        {
            var transaction = new StockMarketSim.Transaction(int.Parse(id), StockMarketSim.TransactionType.Sell, Double.Parse(quantity));

            Money += transaction.Stock_Used.Curr_Price * transaction.Quantity;
            Holdings[transaction.Stock_Used] -= transaction.Quantity;

            transaction.Execute();
            lock (StockMarket.GetTransactions())
            {
                StockMarket.AddTransaction(transaction);
            }
        }
        catch (FormatException e)
        {
            Console.WriteLine("Please input a valid id/quantity number");
            Console.WriteLine(e.Message);
        }
    }

    private static void AddStockToMarket(string stockName, string stockPrice_In_String)
    {
        try
        {
            StockMarketSim.Stock stock = new(stockName, Double.Parse(stockPrice_In_String),StockMarket.GetStocks().Count()+1);
            StockMarket.AddStock(stock);
        }
        catch (FormatException e)
        {
            Console.WriteLine("Please input a valid double for stock quantity");
            Console.WriteLine(e.Message);
        }
    }

    public static void PrintHighestStock()
    {
        var query = StockMarket.GetStocks().OrderByDescending(s => s.Curr_Price).Take(1);
        foreach (var v in query)
        {
            Console.WriteLine(v.PrintStock());
        }
    }

    public static void PrintAllStocks()
    {
        foreach (var v in StockMarket.GetStocks())
        {
            Console.WriteLine(v.PrintStock());
        }
    }

    public static void PrintLowestStock()
    {
        var query = StockMarket.GetStocks().OrderByDescending(s => s.Curr_Price).TakeLast(1);
        foreach (var v in query)
        {
            Console.WriteLine(v.PrintStock());
        }
    }

    public static void PrintStocksSortedByName()
    {
        var query = StockMarket.GetStocks().OrderByDescending(s => s.Name).Take(1);
        foreach (var v in query)
        {
            Console.WriteLine(v.PrintStock());
        }
    }

    public static void PrintTrans()
    {
        lock (StockMarket.GetTransactions())
        {
            foreach (var v in StockMarket.GetTransactions())
            {
                lock (v)
                {
                    Console.WriteLine(v.PrintDetails());
                }
            }
        }

    }

    public static void PrintTransAbove(double price_point)
    {
            var query = StockMarket.GetTransactions().Where(v => v.Quantity > price_point).OrderByDescending(v => v.Quantity);
            foreach (var v in query)
            {
                lock (v)
                {
                    Console.WriteLine(v.PrintDetails());
                }
            }
    }

    public static void PrintTransBelow(double price_point)
    {
            var query = StockMarket.GetTransactions().Where(v => v.Quantity < price_point).OrderByDescending(v => v.Quantity);
            foreach (var v in query)
            {
                lock (v)
                {
                    Console.WriteLine(v.PrintDetails());
                }
            }
    }

    public static void PrintTransById(string stock_id)
    {
        try
        {
                var id = Double.Parse(stock_id);
                var query = StockMarket.GetTransactions().Where(v => v.Stock_Used.Id == id);
                foreach (var v in query)
                {
                    lock (v)
                    {
                        Console.WriteLine(v.PrintDetails());
                    }
                }
        }
        catch (FormatException e)
        {
            Console.WriteLine("Please input a valid id");
            Console.WriteLine(e.Message);
        }
    }

    public static void StockThreads()
    {
        while (true)
        {
            Random r = new Random();

            int buyOrSell = r.Next(2);
            int stockNumber = r.Next(StockMarketSim.StockMarket.GetStocks().Count());
            int quantity = r.Next(1000);

            TransactionType BuyOrSell = TransactionType.Buy;
            if (buyOrSell % 2 == 0) { BuyOrSell = TransactionType.Sell; }

            var t = new StockMarketSim.Transaction(StockMarket.GetStocks().ElementAt(stockNumber).Id, BuyOrSell, quantity);
            t.Execute();

            StockMarket.AddTransaction(t);

            Task.Delay(1000);
        }
    }
}
