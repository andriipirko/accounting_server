using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using WebAPI.Models.Statistic;

namespace WebAPI.Models
{
    public class DbContext
    {
        public string ConnectionString { get; set; }

        public DbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<UserModels.UserFromDbModel> GetAllUsers()
        {
            List<UserModels.UserFromDbModel> list = new List<UserModels.UserFromDbModel>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE uactive = TRUE", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserModels.UserFromDbModel()
                        {
                            uid = reader.GetInt32("uid"),
                            ulogin = reader.GetString("ulogin"),
                            upassword = reader.GetString("upassword"),
                            uactive = reader.GetBoolean("uactive")
                        });
                    }
                }
            }

            return list;
        }

        public UserModels.UserFromDbModel GetCurrentUser(string UserName)
        {
            UserModels.UserFromDbModel _user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE uactive = TRUE AND ulogin = \"" + UserName + "\"", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _user = (new UserModels.UserFromDbModel()
                        {
                            uid = reader.GetInt32("uid"),
                            ulogin = reader.GetString("ulogin"),
                            upassword = reader.GetString("upassword"),
                            uactive = reader.GetBoolean("uactive"),
                            customer = reader.GetBoolean("customer"),
                            realizator = reader.GetBoolean("realizator"),
                            accounter = reader.GetBoolean("accounter"),
                            administrator = reader.GetBoolean("administrator")
                        });
                    }
                }
            }

            return _user;
        }

        public List<Orders.OrderViewModel> GetCurrentOrdersGeneral(int uid)
        {
            List<Orders.OrderViewModel> result = new List<Orders.OrderViewModel>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(string.Format("SELECT orders.oid as 'id', DATE(orders.dt) as 'dt', CONCAT_WS(', ', departments.city, departments.street, departments.housenumber) as 'department', orders.state as 'state' FROM orders INNER JOIN departments ON departments.did = orders.did WHERE orders.uid = {0}", uid), conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Orders.OrderViewModel()
                        {
                            oid = reader.GetInt32("id"),
                            dt = reader.GetString("dt").Substring(0, 10),
                            departmentName = reader.GetString("department"),
                            state = reader.GetBoolean("state")
                        });
                    }
                }
                cmd.Dispose();
            }
            return result;
        }

        public List<Orders.OrderComponent> GetOrderItems(int orderId)
        {
            List<Orders.OrderComponent> result = new List<Orders.OrderComponent>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string storageName = string.Empty;
                MySqlCommand cmd = new MySqlCommand($"SELECT storagename FROM storages WHERE storageid = (SELECT storage FROM orders WHERE oid = {orderId});", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storageName = reader.GetString("storagename");
                    }
                }
                cmd.Dispose();

                cmd = new MySqlCommand($"SELECT {storageName}.pname as 'pname', ordercontent.pquantity as 'pquantity' FROM ordercontent INNER JOIN {storageName} ON {storageName}.pid = ordercontent.pid WHERE ordercontent.oid = {orderId} ", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Orders.OrderComponent()
                        {
                            pname = reader.GetString("pname"),
                            pquantity = reader.GetInt32("pquantity")
                        });
                    }
                }
                cmd.Dispose();
            }
            return result;
        }

        public List<WebAPI.Models.Departments.DepartmentModel> GetAllDepartments()
        {
            List<WebAPI.Models.Departments.DepartmentModel> result = new List<WebAPI.Models.Departments.DepartmentModel>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT did, dname, city FROM departments WHERE dactive = true", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new WebAPI.Models.Departments.DepartmentModel()
                        {
                            did = reader.GetInt32("did"),
                            dname = reader.GetString("dname"),
                            city = reader.GetString("city")
                        });
                    }
                }
                cmd.Dispose();
            }
            return result;
        }

        public bool HideElement(string table, string pid)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(string.Format("UPDATE TABLE{0} SET active = FALSE WHERE pid = {1}", table, pid), conn);
                    cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public List<Storages.StorageModel> GetAllStorages()
        {
            List<Storages.StorageModel> result = new List<Storages.StorageModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT storageid, storagename, storagetitle, sactive, availableToOrder FROM storages;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Storages.StorageModel()
                            {
                                Id = reader.GetInt32("storageid"),
                                Name = reader.GetString("storagename"),
                                Title = reader.GetString("storagetitle"),
                                Available = reader.GetBoolean("availableToOrder"),
                                Active = reader.GetBoolean("sactive")
                            });
                        }
                    }
                    cmd.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<Storages.AllStoragesModel> getAllAvailableStorages()
        {
            List<Storages.AllStoragesModel> result = new List<Storages.AllStoragesModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT storageid, storagetitle FROM storages WHERE sactive = true AND availableToOrder = TRUE;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Storages.AllStoragesModel()
                            {
                                storageId = reader.GetInt32("storageid"),
                                storageName = reader.GetString("storagetitle")
                            });
                        }
                    }
                    cmd.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<Storages.AllStoragesModel> getAllInaccessibleStorages()
        {
            List<Storages.AllStoragesModel> result = new List<Storages.AllStoragesModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT storageid, storagetitle FROM storages WHERE sactive = true AND availableToOrder = FALSE;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Storages.AllStoragesModel()
                            {
                                storageId = reader.GetInt32("storageid"),
                                storageName = reader.GetString("storagetitle")
                            });
                        }
                    }
                    cmd.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool CreateNewStorage(string name, string title, bool availableToOrder)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO storages VALUE (NULL, \"{name}\", \"{title}\", FALSE, {availableToOrder});", conn);
                MySqlCommand cmd2 = new MySqlCommand(string.Format("CREATE TABLE {0} {1}, CONSTRAINT pk_{0} PRIMARY KEY(pid));", name, Resources.Classes.DBCommands.GetCreateStorageTableCode()), conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
                finally
                {
                    cmd.Dispose();
                    cmd2.Dispose();
                }                
            }
            return true;
        }

        public bool InsertProductsIntoStorage(int storageId, List<Storages.StorageProductModel> product)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($"SELECT storagename FROM storages WHERE sactive = true AND storageid = {storageId};", conn);
                    string storageName = "";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            storageName = reader.GetString("storagename");
                        }                        
                    }

                    cmd.Dispose();
                    foreach (var item in product)
                    {
                        cmd = new MySqlCommand(string.Format("INSERT INTO {0} VALUE (NULL, \"{1}\", \"{2}\", {3}, 0, {5});", storageName, item.pname, item.code, item.pprice, item.pquantity, item.active), conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }                    
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool CreateNewAccounting(int storageId, int userId, ICollection<UpdateRequestComponent> updateRequest)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT storagename FROM storages
                                                           WHERE sactive = true
                                                           AND storageid = {storageId};", conn);
                    string storageName = "";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            storageName = reader.GetString("storagename");
                        }
                    }
                    cmd.Dispose();

                    cmd = new MySqlCommand($"INSERT INTO receiving VALUE(NULL, NOW(), {userId}, {storageId});", conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int receivingId = 0;
                    cmd = new MySqlCommand($"SELECT rid FROM receiving ORDER BY rid DESC LIMIT 1;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receivingId = reader.GetInt32("rid");
                        }
                    }
                    cmd.Dispose();

                    cmd.Dispose();
                    foreach (var item in updateRequest)
                    {
                        int quantity = 0;
                        cmd = new MySqlCommand($"SELECT pquantity FROM {storageName} WHERE pid = {item.id}", conn);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                quantity = reader.GetInt32("pquantity");
                            }
                        }
                        cmd.Dispose();

                        cmd = new MySqlCommand($"UPDATE {storageName} SET pquantity = {quantity + item.count} WHERE pid = {item.id}", conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        cmd = new MySqlCommand($"INSERT INTO contentrec VALUE (NULL, {receivingId}, {item.id}, {item.count});", conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool CreateNewOrder(int storageId, ICollection<UpdateRequestComponent> updateRequest, int userId, int departmentId)
        {
            MySqlCommand cmd;
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    cmd = new MySqlCommand($"INSERT INTO orders VALUE (NULL, NOW(), {userId}, {departmentId}, {storageId}, FALSE);", conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int orderId = 0;
                    cmd = new MySqlCommand("SELECT oid FROM orders ORDER BY oid DESC LIMIT 1;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderId = reader.GetInt32("oid");
                        }
                    }

                    foreach (var item in updateRequest)
                    {
                        cmd = new MySqlCommand($"INSERT INTO ordercontent VALUE (NULL, {orderId}, {item.id}, {item.count});", conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public List<Storages.StorageProductModel> GetAllProductsFromStorageById(int storageId)
        {
            List<Storages.StorageProductModel> result = new List<Storages.StorageProductModel>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($"SELECT storagename FROM storages WHERE sactive = true AND storageid = {storageId};", conn);
                    string storageName = "";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            storageName = reader.GetString("storagename");
                        }                        
                    }
                    cmd.Dispose();

                    cmd = new MySqlCommand($"SELECT * FROM {storageName};", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(new Storages.StorageProductModel()
                            {
                                pid = reader.GetInt32("pid"),
                                pname = reader.GetString("pname"),
                                code = reader.GetString("code"),
                                pprice = reader.GetInt32("pprice"),
                                pquantity = reader.GetInt32("pquantity"),
                                active = reader.GetBoolean("active")
                            });
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public void HideStorageById(int id)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($"UPDATE storages SET sactive = FALSE WHERE storageid = {id};", conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public List<GeneralStatisticDto> GetGeneralAccountingStatistic(int storageId, string firstDate, DateTime lastDate)
        {
            lastDate = lastDate.AddDays(1);
            List<GeneralStatisticDto> result = new List<GeneralStatisticDto>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT storagename FROM storages
                                                           WHERE storageid = {storageId};", conn);
                    string storageName = "";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            storageName = reader.GetString("storagename");
                        }
                    }
                    cmd.Dispose();

                    cmd = new MySqlCommand($@"SELECT {storageName}.pname as 'name', SUM(contentrec.pquantity) as 'count'
                                              FROM contentrec
                                              INNER JOIN {storageName} ON {storageName}.pid = contentrec.pid
                                              INNER JOIN receiving ON receiving.rid = contentrec.rid
                                              WHERE receiving.storage = {storageId}
                                              AND receiving.dt >= '{firstDate}'
                                              AND receiving.dt <= '{lastDate.ToString()}'
                                              GROUP BY contentrec.pid;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new GeneralStatisticDto
                            {
                                Name = reader.GetString("name"),
                                Count = reader.GetInt32("count")
                            });
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<GeneralStatisticDto> GetGeneralOrdersStatistic(int storageId, string firstDate, DateTime lastDate)
        {
            lastDate = lastDate.AddDays(1);
            List<GeneralStatisticDto> result = new List<GeneralStatisticDto>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT storagename FROM storages
                                                           WHERE storageid = {storageId};", conn);
                    string storageName = "";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            storageName = reader.GetString("storagename");
                        }
                    }
                    cmd.Dispose();

                    cmd = new MySqlCommand($@"SELECT {storageName}.pname as 'name', SUM(ordercontent.pquantity) as 'count'
                                              FROM ordercontent
                                              INNER JOIN {storageName} ON {storageName}.pid = ordercontent.pid
                                              INNER JOIN orders ON orders.oid = ordercontent.oid
                                              WHERE orders.storage = {storageId}
                                              AND orders.dt >= '{firstDate}'
                                              AND orders.dt <= '{lastDate.ToString()}'
                                              GROUP BY ordercontent.pid;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new GeneralStatisticDto
                            {
                                Name = reader.GetString("name"),
                                Count = reader.GetInt32("count")
                            });
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<ItemComparisionDto> GetStatisticAccountingComparisionItem(DateTime startDate, DateTime endDate, int storageId, int pid)
        {
            endDate.AddDays(1);
            List<ItemComparisionDto> result = new List<ItemComparisionDto>();
            var rangeTimeSpan = endDate.Subtract(startDate);
            var rangeTimeArray = new DateTime[rangeTimeSpan.Days + 1];
            var currentDate = startDate;
            for (int i = 0; i <= rangeTimeSpan.Days; i++)
            {
                rangeTimeArray[i] = currentDate;
                currentDate = currentDate.AddDays(1);
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd;
                    foreach (var item in rangeTimeArray)
                    {
                        cmd = new MySqlCommand($@"SELECT SUM(contentrec.pquantity) as 'sum' FROM contentrec INNER JOIN receiving ON receiving.rid = contentrec.rid WHERE receiving.dt >= '{item.ToString("yyyy-MM-dd")}' AND receiving.dt <= '{item.AddDays(1).ToString("yyyy-MM-dd")}' AND receiving.storage = {storageId} AND contentrec.pid = {pid} GROUP BY contentrec.pid;", conn);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ItemComparisionDto
                                {
                                    Date = item.ToString("dd-MM-yyyy"),
                                    Count = reader.GetInt32("sum")
                                });
                            }
                            if (!reader.HasRows)
                            {
                                result.Add(new ItemComparisionDto
                                {
                                    Date = item.ToString("dd-MM-yyyy"),
                                    Count = 0
                                });
                            }
                        }
                        cmd.Dispose();
                    }                   
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<ItemComparisionDto> GetStatisticOrdersComparisionItem(DateTime startDate, DateTime endDate, int storageId, int pid)
        {
            endDate.AddDays(1);
            List<ItemComparisionDto> result = new List<ItemComparisionDto>();
            var rangeTimeSpan = endDate.Subtract(startDate);
            var rangeTimeArray = new DateTime[rangeTimeSpan.Days + 1];
            var currentDate = startDate;
            for (int i = 0; i <= rangeTimeSpan.Days; i++)
            {
                rangeTimeArray[i] = currentDate;
                currentDate = currentDate.AddDays(1);
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd;
                    foreach (var item in rangeTimeArray)
                    {
                        cmd = new MySqlCommand($@"SELECT SUM(ordercontent.pquantity) as 'sum' FROM ordercontent INNER JOIN orders ON orders.oid = ordercontent.oid WHERE orders.dt >= '{item.ToString("yyyy-MM-dd")}' AND orders.dt <= '{item.AddDays(1).ToString("yyyy-MM-dd")}' AND orders.storage = {storageId} AND ordercontent.pid = {pid} GROUP BY ordercontent.pid;", conn);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ItemComparisionDto
                                {
                                    Date = item.ToString("dd-MM-yyyy"),
                                    Count = reader.GetInt32("sum")
                                });
                            }
                            if (!reader.HasRows)
                            {
                                result.Add(new ItemComparisionDto
                                {
                                    Date = item.ToString("dd-MM-yyyy"),
                                    Count = 0
                                });
                            }
                        }
                        cmd.Dispose();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}