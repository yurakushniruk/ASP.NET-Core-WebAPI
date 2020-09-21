using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public abstract class Resource
    {
        /* 
         * This property will be at the very top of all serialized repsonses.
         * Every resource returned from the API will include an href that
         * contains the absolute uri of the resource itself
         * this self-referencal link takes a place of an id property
         * and it is the RESTful way of exprecing which resource or document
         * the client is currently viewing or maniplating
         */
        [JsonProperty(Order = -2)]
        public string Href { get; set; }
    }
}
