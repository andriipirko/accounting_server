using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Models;
using WebAPI.Models.Storages;
using WebAPI.SignalR;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Storages")]
    public class StoragesController : Controller
    {
        protected IHubContext<ConnectionManager> _context;

        public StoragesController(IHubContext<ConnectionManager> context)
        {
            this._context = context;
        }

        /// <summary>
        /// Метод створює нову таблицю в базі даних.
        /// </summary>
        /// <param name="Name">Назва нової таблиці в БД</param>
        [HttpPost]
        public void Post(string name, string title, bool availableToOrder = false)
        {
            //if (HttpContext.Session.GetInt32("administrator") == 1)
            //{
            //    DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            //    _context.CreateNewStorage(Name);
            //}                
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            _context.CreateNewStorage(name, title, availableToOrder);
        }

        /// <summary>
        /// Внести новий product в таблицю зі вказаним id
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("/api/Storages/insert/{id}", Name = "InsertDataIntoStorage", Order = 1)]
        public void Post([FromBody]StorageProductsModel model)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            _context.InsertProductsIntoStorage(model.id, model.list);
        }

        /// <summary>
        /// Отримати список всіх таблиць, призначених для збереження даних.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/Storages/all/", Name = "AllStorages", Order = 1)]
        public List<Models.Storages.StorageModel> GetAllStorages()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetAllStorages();
        }

        /// <summary>
        /// Отримати список всіх таблиць, призначених для збереження даних і доступних для замовлення.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/Storages/all/available", Name = "AllAvailableStorages", Order = 1)]
        public List<Models.Storages.AllStoragesModel> Get()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.getAllAvailableStorages();
        }

        /// <summary>
        /// Отримати список всіх таблиць, призначених для збереження даних і недоступних для замовлення.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/Storages/all/inaccessible", Name = "AllInaccessibleStorages", Order = 1)]
        public List<Models.Storages.AllStoragesModel> GetInaccessible()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.getAllInaccessibleStorages();
        }

        /// <summary>
        /// Отримати список products із таблиці за вказаним id.
        /// </summary>
        /// <param name="id">ID storage таблиці.</param>
        /// <returns></returns>
        [HttpGet("/api/Storages/all/{id}", Name = "AllProductsFromStorageById", Order = 1)]
        public List<StorageProductModel> Get(int id)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            var result = _context.GetAllProductsFromStorageById(id);
            return result;
        }

        [HttpPost("/api/Storages/CreateAccounting/{id}", Name = "CreateNewAccounting", Order = 1)]
        public bool PostAccountingAsync(int id, string data)
        {
            string[] separated_data = data.Replace("{", string.Empty)
                                          .Replace("}", string.Empty)
                                          .Replace("\"", string.Empty)
                                          .Split(',');
            ICollection<UpdateRequestComponent> requests = new List<UpdateRequestComponent>();
            foreach(var element in separated_data)
            {
                string[] single_component = element.Split(':');
                if (single_component[1] != string.Empty)
                {
                    requests.Add(new UpdateRequestComponent
                                {
                                    id = Int32.Parse(single_component[0]),
                                    count = Int32.Parse(single_component[1])
                                });
                }
            }

            if (requests.Count == 0)
                return false;

            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            var result = _context.CreateNewAccounting(id, 1, requests);
            if (result)
            {
                this._context.Clients.All.InvokeAsync("UpdateStorage", id);
            }
            return result;
        }

        [HttpPost("/api/Storages/CreateOrder/{id}", Name = "CreateNewOrder", Order = 1)]
        public bool PostOrder(int id, string data)
        {
            string[] separated_data = data.Replace("{", string.Empty)
                                          .Replace("}", string.Empty)
                                          .Replace("\"", string.Empty)
                                          .Split(',');
            ICollection<UpdateRequestComponent> requests = new List<UpdateRequestComponent>();
            foreach (var element in separated_data)
            {
                string[] single_component = element.Split(':');
                if (single_component[1] != string.Empty)
                {
                    requests.Add(new UpdateRequestComponent
                    {
                        id = Int32.Parse(single_component[0]),
                        count = Int32.Parse(single_component[1])
                    });
                }
            }

            if (requests.Count == 0)
                return false;

            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.CreateNewOrder(id, requests, 1, 1);
        }

        [HttpDelete("/api/Storages/hide/{id}", Name = "HideStorageById", Order = 1)]
        public void Delete(int id)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            _context.HideStorageById(id);
        }
    }
}
