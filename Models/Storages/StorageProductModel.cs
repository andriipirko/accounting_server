namespace WebAPI.Models.Storages
{
    public class StorageProductModel
    {
        public int pid { get; set; }
        public string pname { get; set; }
        public string code { get; set; }
        public double pprice { get; set; }
        public int pquantity { get; set; }
        public bool active { get; set; }
    }
}