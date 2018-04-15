using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;

namespace WebAPI.Services
{
    public class MySqlService
    {
        public static bool IsInstalled()
        {
            var result = ServiceController.GetServices();
            var result_2 = result.Select(m => m.ServiceName).Where(m => m.ToLower().Contains("mysql"));
            return result_2.ToList().Count != 0;
        }

        public static bool InstallMySql()
        {
            try
            {
                Console.WriteLine("MySQL server installation will start in 5 sec.");
                Thread.Sleep(5000);
                Console.WriteLine("Installing MySQL server on your computer.");

                Process installer = new Process();
                installer.StartInfo.FileName = $@"{Directory.GetCurrentDirectory()}/Resources/CopyMysqlServer.bat";
                installer.StartInfo.UseShellExecute = true;
                installer.StartInfo.Verb = "runas";
                installer.Start();
                installer.WaitForExit();

                Console.WriteLine("MySql server has been successfully installed.\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
