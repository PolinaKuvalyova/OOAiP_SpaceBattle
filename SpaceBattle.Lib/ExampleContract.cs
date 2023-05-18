using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace SpaceBattle.Lib
{
    [DataContract(Name = "ExampleContract", Namespace = "http://example.com")]
    public class ExampleContract
    {
        [DataMember(Name = "type", Order = 1)]
        public string Type { get; set; }

        [DataMember(Name = "game id", Order = 2)]
        public string gameId { get; set; }

        [DataMember(Name = "game item id", Order = 3)]
        public int gameItemId { get; set; }
    }
}
