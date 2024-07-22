using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Repository;

namespace PayrollAPI.Tests.Controller.Repository
{
    public class DataTransferRepositoryTests
    {
        private DBConnect _context;
        private DataRepository _repository;
        private readonly ILogger _logger;

        private async Task<DBConnect> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DBConnect>()
           .UseMySQL("Host=127.0.0.1;Database=payrolldb;Username=admin;Password=Pass#word1")
           .Options;

            _context = new DBConnect(options);
            // _context.Database.EnsureDeleted();
            // _context.Database.Migrate();

            //_repository = new DataRepository(_context);

            return _context;
        }

        [Fact]
        public async void DataRepositoryTests_DataTransfer_OK()
        {
            //Arrange
            string filePath = @"C:\Users\17532\source\repos\pavithra91\PayrollAPI\PayrollAPI.Tests\Data\tempData.json";
            string fileContent = File.ReadAllText(filePath);

            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.DataTransfer(fileContent); ;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Message.Should().BeEquivalentTo("Data Transered Successfully");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void DataRepositoryTests_DataTransfer_Error(string data)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.DataTransfer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        // Confirm Data Transfer
        [Fact]
        public async void DataRepositoryTests_ConfirmDataTransfer_OK()
        {
            //Arrange
            ApprovalDto approvalDto = new ApprovalDto();
            approvalDto.companyCode = 3000;
            approvalDto.period = 202402;
            approvalDto.approvedBy = "3021ITFI";

            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.ConfirmDataTransfer(approvalDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Message.Should().BeEquivalentTo("Data Transered Confirmed");
        }

        [Theory]
        [MemberData(nameof(ApprovalDataError))]
        public async void DataRepositoryTests_ConfirmDataTransfer_Error(ApprovalDto approvalDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.ConfirmDataTransfer(approvalDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        // Reject Data Transfer
        [Fact]
        public async void DataRepositoryTests_RejectDataTransfer_OK()
        {
            //Arrange
            ApprovalDto approvalDto = new ApprovalDto();
            approvalDto.companyCode = 3000;
            approvalDto.period = 202402;
            approvalDto.approvedBy = "3021ITFI";

            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.RollBackTempData(approvalDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Message.Should().BeEquivalentTo("Data Rollback Operation Completed Successfully");
        }

        [Theory]
        [MemberData(nameof(ApprovalDataError))]
        public async void DataRepositoryTests_RejectDataTransfer_Error(ApprovalDto approvalDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = await _dataRepo.RollBackTempData(approvalDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        // Get Data transfer Statistics
        [Theory]
        [InlineData(3000, 202312)]
        [InlineData(3000, 202402)]
        public async void DataRepositoryTests_GetDataTransferStatistics_OK(int companyCode, int period)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = _dataRepo.GetDataTransferStatistics(companyCode, period);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(0, 0)]
        [InlineData(null, 202312)]
        [InlineData(3000, null)]
        [InlineData(3000, 202405)]
        [InlineData(2000, 202312)]
        public async void DataRepositoryTests_GetDataTransferStatistics_Error(int companyCode, int period)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _dataRepo = new DataRepository(dbContext, null);

            //Act
            var result = _dataRepo.GetDataTransferStatistics(companyCode, period);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        public static IEnumerable<object[]> ApprovalDataError => new List<object[]>
        {
            // Blank Object
            new object[] { new ApprovalDto { } },
            // No Company Code
            new object[] { new ApprovalDto { period = 202312, approvedBy = "3021ITFI" } },
            // No Approved By
            new object[] { new ApprovalDto { companyCode = 3000, period = 202312 } },
            // No Period
            new object[] { new ApprovalDto { companyCode = 3000, approvedBy = "3021ITFI" } },
            // Wrong Period
            new object[] { new ApprovalDto { companyCode = 3000, period = 202312, approvedBy = "3021ITFI" } },
            // Already Approved/Rejected
            new object[] { new ApprovalDto { companyCode = 3000, period = 202312, approvedBy = "3021ITFI" } },
        };
    }
}
