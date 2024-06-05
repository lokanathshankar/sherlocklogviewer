// See https://aka.ms/new-console-template for more information
using Flex.LVA.LexerAndParser.Executable;
using Flex.LVA.LexerAndParser.Tests;

using System.Diagnostics;
using System.Net.WebSockets;

using Castle.Components.DictionaryAdapter.Xml;

Console.WriteLine("Hello, World!");
try
{
    new LoggingTest();
    //new TaskFactory().StartNew(() =>
    //{
    //    while (true)
    //    {
    //        try
    //        {
    //            Console.WriteLine(Process.GetCurrentProcess().PrivateMemorySize64 / 1000000);
    //            {
    //                using (ClientWebSocket aSock = new ClientWebSocket())
    //                {
    //                    string aAddress = "ws://localhost:44487/Temp";
    //                    Console.WriteLine($"Trying to connect {aAddress}");
    //                    aSock.ConnectAsync(new Uri(aAddress), CancellationToken.None).Wait();
    //                    Console.WriteLine($"Trying to connect {aAddress}");
    //                    aSock.SendAsync("Ping Alive Message2"u8.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
    //                }
    //            }
    //        }
    //        catch (Exception aEx)
    //        {
    //            Console.WriteLine(aEx.Message);
    //        }

    //        Thread.Sleep(1000);
    //    }
    //}, TaskCreationOptions.LongRunning);
    new UniversalLogParserTests().TestLogSingleLineBenchMark(1000000);
    GC.Collect();
}
catch (Exception aEx)
{
    Console.WriteLine(aEx);
}
Console.ReadKey();