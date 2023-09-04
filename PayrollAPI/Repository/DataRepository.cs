using PayrollAPI.Data;
using PayrollAPI.Interfaces;

namespace PayrollAPI.Repository
{
    public class DataRepository : IData
    {
        private readonly DBConnect _dbConnect;
        public DataRepository(DBConnect db)
        {
            _dbConnect = db;
        }
    }
}
