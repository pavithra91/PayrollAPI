using Microsoft.EntityFrameworkCore;

namespace PayrollAPI.Data
{
    public class DBConnect: DbContext
    {
        public DBConnect(DbContextOptions<DBConnect> options) : base(options)
        {

        }
    }
}
