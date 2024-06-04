using OrionPro;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BolidSoap
{
  internal class Program
  {
    static void WriteJson<T>(T o, string sec_name)
    {
      var s = JsonSerializer.Serialize(o,
        new JsonSerializerOptions
        {
          Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
          WriteIndented = true
        }
        );
      Console.WriteLine("==========================");
      Console.WriteLine(typeof(T).Name);
      Console.WriteLine("==========================");
      Console.WriteLine(s);

      File.AppendAllText("log.json", "\n");
      File.AppendAllText("log.json", "==========================\n");
      File.AppendAllText("log.json", $"{sec_name} of {typeof(T).Name}");
      File.AppendAllText("log.json", "\n==========================\n");
      File.AppendAllText("log.json", s);
    }
    public static string compute_md5(string input)
    {
      // Use input string to calculate MD5 hash
      using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +

        // Convert the byte array to hexadecimal string prior to .NET 5
        // StringBuilder sb = new System.Text.StringBuilder();
        // for (int i = 0; i < hashBytes.Length; i++)
        // {
        //     sb.Append(hashBytes[i].ToString("X2"));
        // }
        // return sb.ToString();
      }
    }
    static void Main(string[] args)
    {
      File.WriteAllText("log.json", "");
      File.WriteAllText("log.xml", "");
      var token = string.Empty;
      Console.WriteLine("Hello, Orion!");
      var client = new OrionProClient();
      client.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());

      var md5_pass = compute_md5("1");
      token = client.GetLoginTokenAsync("administrator", md5_pass).Result.@return.OperationResult;

      //var computers = client.GetComputersAsync(token).Result.@return.OperationResult;
      //WriteJson(computers, nameof(computers));

      //var comports = client.GetComPortsAsync(token).Result.@return.OperationResult;
      //WriteJson(comports, nameof(comports));

      //var devices = client.GetDevicesAsync(string.Empty).Result.@return.OperationResult;
      //WriteJson(devices, nameof(devices));

      //TDeviceItem[]? devices_items = null;
      //foreach (var device in devices)
      //{
      //  devices_items = client.GetDeviceItemsAsync(device.Id, token).Result.@return.OperationResult;
      //  WriteJson(devices_items, $"for device {device.Name}[{device.Id}]->{nameof(devices_items)}");
      //}

      //var items = devices_items?.Select(x => new TItem() 
      //{ 
      //  ItemId = x.Id,
      //  ItemType = x.ItemType
      //}
      //).ToArray();

      var items_result = client.GetItemsAsync(0, 5, token).Result.@return.OperationResult;
      
      var items_result1 = new List<TItem>();

      foreach (var item in items_result!)
      {
        item.Timestamp = item.Timestamp + new TimeSpan(1, 1, 1);
        items_result1.Add(item);
      }
      var items_result2 = client.GetItemsStatesAsync(token, items_result.ToArray()).Result.@return.OperationResult;
      //var sections = client.GetSectionsAsync(true, 0, 5, token).Result.@return.OperationResult;
      //WriteJson(sections, nameof(sections));

      //var sections_groups = client.GetSectionsGroupsAsync(true, 0, 5, token).Result.@return.OperationResult;
      //WriteJson(sections_groups, nameof(sections_groups));

      //var items = client.GetItemsAsync(0, 50, token).Result.@return.OperationResult;
      //WriteJson(items, nameof(items));

      //var devicesLoops = client.GetDevicesItemsAsync(["LOOP"], token).Result.@return.OperationResult;
      //WriteJson(devicesLoops, nameof(devicesLoops));

      //var devicesRelays = client.GetDevicesItemsAsync(["RELAY"], token).Result.@return.OperationResult;
      //WriteJson(devicesRelays, nameof(devicesRelays));
    }
  }
}
