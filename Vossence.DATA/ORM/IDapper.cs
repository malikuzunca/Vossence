using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.ORM
{
    public interface IDapper : IDisposable
    {
        List<T> QueryApp<T>(string sql);
        T Get<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure);
        List<T> GetAll<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure);
        T Insert<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure);
        T Update<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure);
        T Delete<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure);
    }
}
