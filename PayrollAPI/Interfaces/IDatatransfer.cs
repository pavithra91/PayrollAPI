using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces
{
    public interface IDatatransfer
    {
        public MsgDto ConfirmDataTransfer(int period);
    }
}
