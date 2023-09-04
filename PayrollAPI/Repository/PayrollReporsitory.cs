using PayrollAPI.Data;
using PayrollAPI.Interfaces;

namespace PayrollAPI.Repository
{
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _dbConnect;

        public PayrollReporsitory(DBConnect db)
        {
            _dbConnect = db;
        }

        //public void Process
    }
}
