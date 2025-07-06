using PocketBaseSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace Example.Models
{
    public class Todo : BaseModel
    {
        public string? name { get; set; }

        public List<Entry>? Entries { get; set; }
    }
}
