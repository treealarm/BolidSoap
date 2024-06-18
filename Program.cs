using OrionPro;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BolidSoap
{
  internal class Program
  {
    static async Task GetItemsStates()
    {
      //{
      //  "ItemType": "LOOP",
      //  "ItemId": 275,
      //  "Rights": 0,
      //  "State": 250,
      //  "ComputerId": -1,
      //  "OwnerId": -1,
      //  "Timestamp": "2024-06-04T12:37:51.39+03:00"
      //}
      //var items_req = new List<TItem>();
      //items_req.Add(new TItem()
      //{
      //  ItemType = "SECTIONGROUP",
      //  ItemId = 1
      //});
      //var items_res2 = await m_client.GetItemsStatesAsync(m_token, items_req?.ToArray());

    }


    static async Task  Main(string[] args)
    {
      Console.WriteLine("Hello, Orion!");

      try
      {
        var poller = new OrionPoller(
                "http://10.1.50.29:8090/soap/IOrionPro",
                "administrator",
                "1");

        await poller.Init();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        Console.WriteLine("=================================");
      }

      /*var items_res2 = m_client.GetItemsStatesAsync(m_token, items_req?.ToArray()).Result.@return.OperationResult;

      if (items_res2.Length > 0)
      {
        WriteJson(items_res2, $"[items_res2]->{nameof(items_res2)}");
      }

      items_res2 = m_client.ControlItemsAsync(m_token, items_req?.ToArray(), 2, 0, 0, 0).Result.@return.OperationResult;
      if (items_res2.Length > 0)
      {
        WriteJson(items_res2, $"[items_res2_control]->{nameof(items_res2)}");
      }*/
      //int offset = 0;
      //int pack = 100;

      //var items_result = client.GetItemsAsync(offset, pack, token).Result.@return.OperationResult;

      //while (items_result.Length > 0)
      //{
      //  var items_result2 = client.GetItemsStatesAsync(token, items_result.ToArray()).Result.@return.OperationResult;

      //  if (items_result2.Length > 0)
      //  {
      //    WriteJson(items_result2, $"items_result2]->{nameof(items_result2)}");
      //  }


      //  items_result = client.GetItemsAsync(offset, pack, token).Result.@return.OperationResult;
      //  offset += pack;
      //}

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
