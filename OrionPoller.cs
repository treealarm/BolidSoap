﻿using OrionPro;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BolidSoap
{
  internal class OrionPoller
  {
    private string _token = string.Empty;
    private OrionProClient? _client;
    private readonly string _remoteAddress;
    private readonly string _user;
    private readonly string _password;
    public bool IsInited
    {
      get;
      private set;
    } = false;

    private ConcurrentDictionary<string, TComputer> _computers = new ConcurrentDictionary<string, TComputer>();
    private ConcurrentDictionary<string, TDevice> _devices = new ConcurrentDictionary<string, TDevice>();
    private ConcurrentDictionary<string, TDeviceItem> _device_items = new ConcurrentDictionary<string, TDeviceItem>();
    private ConcurrentDictionary<string, TSection> _section_items = new ConcurrentDictionary<string, TSection>();
    private ConcurrentDictionary<string, TSectionsGroup> _sections_group_items = new ConcurrentDictionary<string, TSectionsGroup>();
    private ConcurrentDictionary<string, TItem> _item_states = new ConcurrentDictionary<string, TItem>();
    private ConcurrentDictionary<string, TEventType> _event_types = new ConcurrentDictionary<string, TEventType>();
    public OrionPoller(string remoteAddress, string user, string password)
    {
      _remoteAddress = remoteAddress;
      _user = user;
      _password = password;
    }

    private async Task SaveAll()
    {
      File.WriteAllText($"{nameof(_computers)}", "");
      File.WriteAllText($"{nameof(_devices)}", "");
      File.WriteAllText($"{nameof(_device_items)}", "");
      File.WriteAllText($"{nameof(_section_items)}", "");
      File.WriteAllText($"{nameof(_sections_group_items)}", "");
      File.WriteAllText($"{nameof(_item_states)}", "");
      File.WriteAllText($"{nameof(_event_types)}", "");

      await WriteJson(_computers, string.Empty, $"{nameof(_computers)}.json");
      await WriteJson(_devices, string.Empty, $"{nameof(_devices)}.json");
      await WriteJson(_device_items, string.Empty, $"{nameof(_device_items)}.json");
      await WriteJson(_section_items, string.Empty, $"{nameof(_section_items)}.json");
      await WriteJson(_sections_group_items, string.Empty, $"{nameof(_sections_group_items)}.json");
      await WriteJson(_item_states, string.Empty, $"{nameof(_item_states)}.json");
      await WriteJson(_event_types, string.Empty, $"{nameof(_event_types)}.json");
    }
    private static string compute_md5(string input)
    {
      using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
      }
    }

    static async Task  WriteJson<T>(T o, [CallerMemberName] string sec_name = "", string file_name = "")
    {
      var s = JsonSerializer.Serialize(o,
        new JsonSerializerOptions
        {
          Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
          WriteIndented = true
        }
      );
      //Console.WriteLine($"start======{sec_name}====================");
      //Console.WriteLine(typeof(T).Name);
      //Console.WriteLine("==========================");

      //Console.WriteLine(s);

      //Console.WriteLine($"end======{sec_name}====================");

      if (!string.IsNullOrEmpty(sec_name))
      {
        File.AppendAllText(file_name, "\n");
        File.AppendAllText(file_name, "==========================\n");
        File.AppendAllText(file_name, $"{sec_name} of {typeof(T).Name}");
        File.AppendAllText(file_name, "\n==========================\n");
      }
      
      await File.AppendAllTextAsync(file_name, s);
    }

    public async Task<bool> Init()
    {      
      File.WriteAllText("log.json", "");
      File.WriteAllText("log.xml", "");

      _client = new OrionProClient(new OrionProClient.EndpointConfiguration(), _remoteAddress);
      _client.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());

      bool retVal = await CreateToken();

      retVal = retVal && await GetComputers();
      retVal = retVal && await GetDevices();
      retVal = retVal && await GetDeviceItems();

      retVal = retVal && await GetSections();
      retVal = retVal && await GetSectionsGroups();
      retVal = retVal && await GetEventTypes();

      await SaveAll();
      IsInited = retVal;

      return retVal;
    }

    
    private async Task<bool> CreateToken()
    {
      var md5_pass = compute_md5(_password);
      var result = await _client!.GetLoginTokenAsync(_user, md5_pass);
      PrintStatus(result?.@return?.ServiceError);

      if (result?.@return != null && result.@return.Success)
      {
        _token = result.@return.OperationResult;
      }        

      return result?.@return != null && result.@return.Success;
    }
    async Task<bool> GetSections()
    {
      int offset = 0;
      int count = 1000;
      var result = await _client!.GetSectionsAsync(false, offset, count, _token);
      PrintStatus(result?.@return?.ServiceError);

      _section_items.Clear();

      while (result?.@return != null && result.@return.Success)
      {
        var items = result.@return.OperationResult;

        if (items.Length == 0) 
        { 
          break; 
        }
        foreach (var item in items)
        {
          _section_items.AddOrUpdate($"{item.Id}", item, (key, oldValue) => item);
        }
        result = await _client!.GetSectionsAsync(false, offset, count, _token);
        offset += count;
      }
      
      return result?.@return != null && result.@return.Success;
    }

    async Task<bool> GetSectionsGroups()
    {
      int offset = 0;
      int count = 1000;
      var result = await _client!.GetSectionsGroupsAsync(false, offset, count, _token);
      PrintStatus(result?.@return?.ServiceError);

      _sections_group_items.Clear();

      while (result?.@return != null && result.@return.Success)
      {
        var items = result.@return.OperationResult;

        if (items.Length == 0)
        {
          break;
        }
        foreach (var item in items)
        {
          _sections_group_items.AddOrUpdate($"{item.Id}", item, (key, oldValue) => item);
        }
        result = await _client!.GetSectionsGroupsAsync(false, offset, count, _token);
        offset += count;
      }

      return result?.@return != null && result.@return.Success;
    }
    private void PrintStatus(TServiceError? error)
    {
      if (error == null)
      {
        return;
      }
      Console.WriteLine($"{error.Description}[{error.ErrorCode}]");
    }
    async Task<bool> GetComputers()
    {
      var result = await _client!.GetComputersAsync(_token);
      PrintStatus(result?.@return?.ServiceError);

      if (result?.@return != null && result.@return.Success)
      {
        _computers.Clear();

        var items = result.@return.OperationResult;

        foreach (var item in items)
        {
          _computers.AddOrUpdate($"{item.Id}", item, (key, oldValue) => item);
        }
      }      

      return result?.@return != null && result.@return.Success;
    }
    async Task<bool> GetDevices()
    {
      var result = await _client!.GetDevicesAsync(_token);
      PrintStatus(result?.@return?.ServiceError);

      if (result?.@return != null && result.@return.Success)
      {
        _devices.Clear();

        var items = result.@return.OperationResult;

        foreach (var item in items)
        {
          _devices.AddOrUpdate($"{item.Id}", item, (key, oldValue) => item);
        }
      }

      return result?.@return != null && result.@return.Success;
    }
    async Task<bool> GetEventTypes()
    {
      var result = await _client!.GetEventTypesAsync(_token);
      PrintStatus(result?.@return?.ServiceError);

      if (result?.@return != null && result.@return.Success)
      {
        _event_types.Clear();

        var items = result.@return.OperationResult;

        foreach (var item in items)
        {
          _event_types.AddOrUpdate($"{item.Id}", item, (key, oldValue) => item);
        }
      }

      return result?.@return != null && result.@return.Success;
    }

    async Task<bool> GetDeviceItems()
    {
      TDevice[] devices = _devices.Values.ToArray();

      List<TDeviceItem> devices_items = new List<TDeviceItem>();
      _device_items.Clear();

      foreach (var device in devices)
      {
        var result = await _client!.GetDeviceItemsAsync(device.Id, _token);
        PrintStatus(result?.@return?.ServiceError);

        if (result?.@return == null || !result.@return.Success)
        {
          return false;      
        }
        var items = result.@return.OperationResult;

        Parallel.ForEach(items, item =>
        {
        _device_items.AddOrUpdate($"{item.ItemType}{item.Id}", item, (key, oldValue) => item);
        });
      }
      return true;
    }

    List<TItem> ConvertToItems(List<TDeviceItem> tItems)
    {
      List<TItem> items = new List<TItem>();

      foreach(var item in tItems)
      {
        items.Add(new TItem()
        {
          ItemType = item.ItemType,
          ItemId = item.Id
        });
      }
      return items;
    }
    List<TItem> ConvertToItems(List<TSection> tItems)
    {
      List<TItem> items = new List<TItem>();

      foreach (var item in tItems)
      {
        items.Add(new TItem()
        {
          ItemType = "SECTION",
          ItemId = item.Id
        });
      }
      return items;
    }
    List<TItem> ConvertToItems(List<TSectionsGroup> tItems)
    {
      List<TItem> items = new List<TItem>();

      foreach (var item in tItems)
      {
        items.Add(new TItem()
        {
          ItemType = "SECTIONGROUP",
          ItemId = item.Id
        });
      }
      return items;
    }
    async Task<bool> GetItemsStates(List<TItem> items_req)
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

      var result = await _client!.GetItemsStatesAsync(_token, items_req?.ToArray());
      PrintStatus(result?.@return?.ServiceError);

      if (result?.@return != null && result.@return.Success)
      {
        var items = result.@return.OperationResult;
        Parallel.ForEach(items, item =>
        {
          _item_states.AddOrUpdate(
            $"{item.ItemType}{item.ItemId}",
            item,
            (key, oldValue) => item);
        });
      }
      return result?.@return != null && result.@return.Success;
    }
    public async Task<bool> Poll()
    {
      var retVal = true;
      retVal = retVal && await GetItemsStates(ConvertToItems(_device_items.Values.ToList()));
      retVal = retVal && await GetItemsStates(ConvertToItems(_section_items.Values.ToList()));
      retVal = retVal && await GetItemsStates(ConvertToItems(_sections_group_items.Values.ToList()));

      return retVal;
    }

    public void PrintStates()
    {
      var states = _item_states.Values.ToList();

      foreach (var item in states)
      {
        Console.WriteLine($"{item.ItemType}{item.ItemId} = {item.State}");
      }
    }
  }
}
