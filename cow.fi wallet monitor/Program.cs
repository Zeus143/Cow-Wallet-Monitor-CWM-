using cow.fi_wallet_monitor;

internal class Program
{
    static List<CowAPI.walletRes> previousOrders = new();
    static List<CowAPI.tokens> tokens = CowAPI.Get.Wallets().Result;
    

    /* FARKLI WALLET İÇİN BURAYI DEĞİŞTİREBİLİRSİNİZ */
    public static string wallet = "0x5be9a4959308a0d0c7bc0870e319314d8d957dbb";
    public static int limit = 10;

    public static void Main(string[] args)
    {
        
        previousOrders = CowAPI.Check.Wallet(wallet: wallet, limit: limit).Result;
        int requests = 0;
        while (true)
        {
            var currentOrders = CowAPI.Check.Wallet(wallet: wallet, limit: limit).Result;

            var newOrders = currentOrders.Where(current =>
                !previousOrders.Any(prev =>
                    prev.date == current.date &&
                    prev.sellToken == current.sellToken &&
                    prev.buyToken == current.buyToken &&
                    prev.sellAmount == current.sellAmount &&
                    prev.buyAmount == current.buyAmount
                )).ToList();

            if (newOrders.Any())
            {
                Console.WriteLine("\nYeni işlemler bulundu:");
                foreach (var order in newOrders)
                {
                    if (order.status.Contains("fill"))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = $"https://www.binance.com/en-TR/futures/{tokens.FirstOrDefault(token => token.adress == order.buyToken)?.symbol}USDT",
                            UseShellExecute = true
                        });
                    }
                    Console.WriteLine($"Tarih: {order.date}");
                    Console.WriteLine($"Satış Token: {order.sellToken}");
                    Console.WriteLine($"Alış Token: {order.buyToken}");
                    Console.WriteLine($"Satış Miktarı: {order.sellAmount}");
                    Console.WriteLine($"Alış Miktarı: {order.buyAmount}");
                    Console.WriteLine($"Durum: {order.status}");
                    Console.WriteLine($"Tür: {order.kind}");
                    Console.WriteLine("------------------------");
                }
            }
            previousOrders = currentOrders;
            requests++;
            Console.Title = $"Total Requests: {requests}";
            Thread.Sleep(100); /* 100ms */
        }
    }
}

