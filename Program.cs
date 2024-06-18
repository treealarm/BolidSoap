using OrionPro;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BolidSoap
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine("Hello, Orion!");

      try
      {
        var poller = new OrionPoller(
                Environment.GetEnvironmentVariable("ORION_ADDR"),
                Environment.GetEnvironmentVariable("ORION_USER"),
                Environment.GetEnvironmentVariable("ORION_PASSWORD"));

        while (!await poller.Init())
        {
          Console.WriteLine("Init failed");
          await Task.Delay(1000);
        }

        Console.WriteLine("Init OK");

        while (true)
        {
          try
          {
            bool retVal = await poller.Poll();
            if (retVal)
            {
              Console.WriteLine("=================================");
              poller.PrintStates();
              Console.WriteLine("=================================");
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.ToString());
          }

          await Task.Delay(5000);
        }

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        Console.WriteLine("=================================");
      }
    }    
  }
}
