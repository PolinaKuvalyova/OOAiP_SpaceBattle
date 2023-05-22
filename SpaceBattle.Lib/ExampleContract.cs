using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace SpaceBattle.Lib;

[DataContract]
public class Contract
{
    [DataMember]
    public JsonDictionary Registration { get; set; }
}

[Serializable]
public class JsonDictionary : ISerializable
{
    // The value to serialize.
    private Dictionary<string, object> m_entries;

    public JsonDictionary()
    {
        m_entries = new Dictionary<string, object>();
    }

    public IEnumerable<KeyValuePair<string, object>> Entries
    {
        get { return m_entries; }
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        foreach (var entry in m_entries)
        {
            info.AddValue(entry.Key, entry.Value);
        }
    }
    protected JsonDictionary(SerializationInfo info, StreamingContext context)
    {
        m_entries = new Dictionary<string, object>();
        foreach (var entry in info)
        {
            m_entries.Add(entry.Name, entry.Value);
        }
    }
}