using PocketBaseSharp.Models;

namespace Example.Models
{
    public class Entry : BaseModel
    {
        public string? name { get; set; }
        public bool is_done { get; set; }
        public string? todo_id { get; set; }

    }
}
