using Chatbot.API;
using Microsoft.SemanticKernel.Connectors.InMemory;
using System.IO;
using System.Text.Json;

public sealed class PersistentInvoiceStore
{
  private readonly InMemoryCollection<Guid, InvoiceVectorStoreRecord> _collection;
  private readonly HashSet<Guid> _ids = new();

  public PersistentInvoiceStore(InMemoryCollection<Guid, InvoiceVectorStoreRecord> collection)
  {
    _collection = collection;
  }

  // Call this whenever you add or update a record
  public async Task UpsertAsync(InvoiceVectorStoreRecord record)
  {
    await _collection.UpsertAsync(record);
    _ids.Add(record.Id);
  }

  public void Delete(Guid id)
  {
    _collection.DeleteAsync(id); // or DeleteAsync(...)
    _ids.Remove(id);
  }

  public void SaveToFile(string filePath)
  {
    var payload = new
    {
      ids = _ids.ToArray(),
      // Pull each record by ID since we can't enumerate the collection
      records = _ids
            .Select(async id => new { id, record = await _collection.GetAsync(id) })
            .ToArray()
    };

    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(filePath, json);
  }

  public async Task LoadFromFileAsync(string filePath)
  {
    if (!File.Exists(filePath))
      return;

    var json = File.ReadAllText(filePath);
    var payload = JsonSerializer.Deserialize<Payload>(json)!;

    _ids.Clear();
    foreach (var id in payload.ids)
      _ids.Add(id);

    foreach (var item in payload.records)
    {
      await _collection.UpsertAsync(item.record);
    }
  }

  public int Length => _ids.Count();


  private record Payload(Guid[] ids, RecordItem[] records);
  private record RecordItem(Guid id, InvoiceVectorStoreRecord record);
}
