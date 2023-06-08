using System;
namespace StockMarketSim
{
    public class Stock
    {

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Curr_Price { get; private set; }
        public DateTime Time_Updated { get; private set; }

        // Basic constructor to create a nameless stock
        public Stock()
        {
            Name = "";
            Curr_Price = 0;
            Time_Updated = DateTime.Now;
        }

        // Constructor to create a custom stock
        public Stock(string name, double curr_price, int id)
        {
            Id = id;
            this.Name = name;
            this.Curr_Price = curr_price;
            Time_Updated = DateTime.Now;
        }

        // Standard copy constructor
        public Stock(Stock stock)
        {
            this.Id = stock.Id;
            this.Name = stock.Name;
            this.Curr_Price = stock.Curr_Price;
            this.Time_Updated = stock.Time_Updated;
        }

        // Logic for buying a stock
        internal void Buy(double quantity)
        {
            lock (this)
            {
                Curr_Price += (quantity);
                this.Time_Updated = DateTime.Now;
            }
        }

        // Logic for selling a stock
        internal void Sell(double quantity)
        {
            lock (this)
            {
                if ((Curr_Price -= (quantity)) <= 0)
                {
                    Buy(quantity);
                    return;
                }

                Curr_Price -= (quantity);
                this.Time_Updated = DateTime.Now;
            }
        }

        private double Calculate_Impact(double quantity)
        {
            Random r = new Random();

            // Generates a value between 0 exclusive and 1 inclusive.
            double Impact_Value = (r.Next(10) + 1) / 10;

            var query = StockMarket.GetTransactions().Where(t => t.Executed == true)
                .OrderByDescending(t => t.Stock_Used.Time_Updated)
                .Take(10);

            double Sum = 0;
            double Count = 0;

            foreach (var t in query)
            {
                Sum += t.Quantity;
                Count++;
            }

            double Average = (Sum / Count);

            return Average * Impact_Value;
        }

        public string PrintStock()
        {
            return $"Id:{Id}, Name:{Name}, Price:{Curr_Price}, Last Updated:" + Time_Updated.ToString();
        }
    }
}