using System;

namespace StockMarketSim
{
    public class StockMarket
    {
        private static List<Stock> Stock_List = new List<Stock>();
        private static List<Transaction> Transaction_List = new List<Transaction>();

        public static void AddStock(Stock s)
        {
            lock (Stock_List)
            {
                Stock_List.Add(s);
            }
        }

        public static void AddTransaction(Transaction t)
        {
            lock (Transaction_List)
            {
                Transaction_List.Add(t);
            }
        }

        public static List<Stock> GetStocks()
        {
            return Stock_List;
        }

        public static List<Transaction> GetTransactions()
        {
            lock (Transaction_List)
            {
                return new List<Transaction>(Transaction_List);
            }
        }

    }
}

