using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Resources.Classes
{
    public class DBCommands
    {
        private static string _createStorageTable = @"(pid INT UNSIGNED AUTO_INCREMENT,
                                             pname VARCHAR(100) NOT NULL,
                                             code TEXT NOT NULL,
                                             pprice DOUBLE NOT NULL DEFAULT 0,
                                             pquantity INT UNSIGNED DEFAULT 0,
                                             active BOOL DEFAULT TRUE";

        public static string GetCreateStorageTableCode() => _createStorageTable;
    }
}
