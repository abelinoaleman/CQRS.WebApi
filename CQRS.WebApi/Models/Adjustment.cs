using System.Collections.Generic;

namespace CQRS.WebApi.Models
{
 

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CostCenter
    {
        public int id { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public int quantity { get; set; }
        public string type { get; set; }
        public int unitCost { get; set; }
    }

    public class Adjustment
    {
        public string date { get; set; }
        public string observations { get; set; }
        public Warehouse warehouse { get; set; }
        public List<Item> items { get; set; }
        public CostCenter costCenter { get; set; }
    }

    public class Warehouse
    {
        public string id { get; set; }
    }


}
