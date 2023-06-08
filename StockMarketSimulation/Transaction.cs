using System;
namespace StockMarketSim
{
    public class Transaction
    {
        readonly public Stock Stock_Used;
        readonly public TransactionType Type;
        readonly public double Quantity;
        public bool Executed { get; private set; }

        public Transaction(int id, TransactionType type, double quantity)
        {
            Stock_Used = find(id);
            Type = type;
            Quantity = quantity;
        }

        private Stock? find(int id)
        {
            List<Stock> Stocks = StockMarket.GetStocks();
            foreach (Stock stock in Stocks)
            {
                if (stock.Id == id)
                {
                    return stock;
                }
            }

            return null;
        }

        public void Execute()
        {
            if (Stock_Used == null)
            {
                throw new Exception("Transaction could not be executed");
            }

            lock (Stock_Used)
            {
                if (Type == TransactionType.Buy)
                {
                    Stock_Used.Buy(Quantity);
                }
                else
                {
                    Stock_Used.Sell(Quantity);
                }
            }

            Executed = true;

        }

        public String PrintDetails()
        {
            if (Type == TransactionType.Buy)
            {
                return "Buy " + Stock_Used.Name + $" for {Quantity} shares";
            }
            else
            {
                return "Sell " + Stock_Used.Name + $" for {Quantity} shares";
            }
        }
    }
}

