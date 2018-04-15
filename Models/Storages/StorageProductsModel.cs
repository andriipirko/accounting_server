using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Storages
{
    public class StorageProductsModel
    {
        public int id;
        public List<Models.Storages.StorageProductModel> list { get; set; }
    }
}
