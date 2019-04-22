using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;

namespace Gjallarhorn.Db
{
    
    public class GjallarhornDbBase
    {
        private readonly string _dbLocation;
        private readonly DynaSql _dynaSql;
        public GjallarhornDbBase(IFileSystem fileSystem)
        {
            _dbLocation = fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gjallarhorn.sqllite");
            _dynaSql = new DynaSql($"Data Source={_dbLocation};Version=3;");//will autocreate db if missing.

        }
    }
}
