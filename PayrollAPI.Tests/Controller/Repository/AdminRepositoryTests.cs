using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Repository;

namespace PayrollAPI.Tests.Controller.Repository
{
    public class AdminRepositoryTests
    {
        private DBConnect _context;
        private AdminRepository _repository;
        private async Task<DBConnect> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DBConnect>()
           .UseMySQL("Host=127.0.0.1;Database=payrolldb;Username=admin;Password=Pass#word1")
           .Options;

            _context = new DBConnect(options);
            // _context.Database.EnsureDeleted();
            // _context.Database.Migrate();

            _repository = new AdminRepository(_context);

            return _context;
        }

        #region Pay Codes
        [Theory]
        [MemberData(nameof(PayCodeDataList))]
        public async void AdminRepositoryTests_Add_ReturnsBadRequest(PayCodeDto payCodeDto)
        {
            //Arrange
            MsgDto msg = new MsgDto()
            {
                MsgCode = 'E',
            };

            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.CreatePayCode(payCodeDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        [Fact]
        public async void AdminRepositoryTests_Add_ReturnsOK()
        {
            //Arrange
            var _payCode = new PayCodeDto()
            {
                companyCode = 3000,
                calCode = "_111",
                description = "Basic Salary",
                payCode = 111,
                rate = 1,
                taxationType = "IT",
                createdBy = "3021ITFI",
                createdDate = new DateTime(),
            };

            MsgDto msg = new MsgDto()
            {
                Data = null,
                MsgCode = 'S',
                Message = "Pay Code Created Successfully",
                Description = null,
            };

            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.CreatePayCode(_payCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.Should().BeEquivalentTo(msg);
        }

        [Fact]
        public async void AdminRepositoryTests_Get_PayCodesOK()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetPayCodes();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(15)]
        public async void AdminRepositoryTests_Get_PayCodesByIdOK(int payCode)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetPayCodesById(payCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(999)]
        [InlineData(555)]
        public async void AdminRepositoryTests_Get_PayCodesById_Null_OK(int payCode)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetPayCodesById(payCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
            result.Data.Should().NotBeNull();
        }


        // Update
        [Theory]
        [MemberData(nameof(UpdatePayCodeNotFoundDataList))]
        public async void AdminRepositoryTests_Update_PayCode_NotFound(PayCodeDto payCodeDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdatePayCode(payCodeDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('N');
            result.Message.Should().BeEquivalentTo("No Pay Code Found");
        }

        [Theory]
        [MemberData(nameof(UpdatePayCodeDataList))]
        public async void AdminRepositoryTests_Update_PayCode_OK(PayCodeDto payCodeDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdatePayCode(payCodeDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
        }

        [Theory]
        [MemberData(nameof(UpdatePayCodeErrorDataList))]
        public async void AdminRepositoryTests_Update_PayCode_Error(PayCodeDto payCodeDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdatePayCode(payCodeDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        #endregion

        #region Calculation
        [Fact]
        public async void AdminRepositoryTests_Get_Calculations_OK()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetCalculations();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async void AdminRepositoryTests_Get_CalculationById_OK(int payCode)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetCalculationsById(payCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(999)]
        [InlineData(555)]
        public async void AdminRepositoryTests_Get_CalculationById_Null_OK(int payCode)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetCalculationsById(payCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
            result.Message.Should().BeEquivalentTo("No Data Available");
        }

        // Add
        [Fact]
        public async void AdminRepositoryTests_Add_Calculations_OK()
        {
            //Arrange
            var _cal = new CalDto()
            {
                companyCode = 3000,
                sequence = 10,
                payCode = 111,
                calCode = "_111",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Basic Salary",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime(),
            };

            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.CreateCalculation(_cal);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Message.Should().BeEquivalentTo("Calculation Created Successfully");
        }

        [Theory]
        [MemberData(nameof(CalculationDataList))]
        public async void AdminRepositoryTests_Add_Calculations_Error(CalDto calDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.CreateCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        //Update
        [Theory]
        [MemberData(nameof(UpdateCalculationDataList))]
        public async void AdminRepositoryTests_Update_Calculation_OK(CalDto calDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdateCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
        }

        [Theory]
        [MemberData(nameof(UpdateCalculationNotFoundDataList))]
        public async void AdminRepositoryTests_Update_Calculation_NotFound(CalDto calDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdateCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('N');
            result.Message.Should().BeEquivalentTo("No Calculation Formula Found");
        }

        [Theory]
        [MemberData(nameof(UpdateCalculationErrorDataList))]
        public async void AdminRepositoryTests_Update_Calculation_Error(CalDto calDto)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.UpdateCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
        }

        //Delete
        [Theory]
        [InlineData(12)]
        public async void AdminRepositoryTests_Delete_Calculation_OK(int id)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);
            CalDto calDto = new CalDto();
            calDto.id = id;

            //Act
            var result = await _adminRepo.DeleteCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Message.Should().BeEquivalentTo("Calculation Mark for Deletion");
        }

        [Theory]
        [InlineData(500)]
        [InlineData(150)]
        public async void AdminRepositoryTests_Delete_Calculation_NotFound(int id)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);
            CalDto calDto = new CalDto();
            calDto.id = id;

            //Act
            var result = await _adminRepo.DeleteCalculation(calDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('N');
            result.Message.Should().BeEquivalentTo("No Calculation Formula Found");
        }

        // Get OT details
        [Theory]
        [InlineData(3000, 202312)]
        public async void AdminRepositoryTests_Get_OTDetails_OK(int companyCode, int period)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetOTDetails(period, companyCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('S');
            result.Data.Should().NotBeNull();
        }

        [Theory]
        [InlineData(3000, 202404)]
        public async void AdminRepositoryTests_Get_OTDetails_NotFound(int companyCode, int period)
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var _adminRepo = new AdminRepository(dbContext);

            //Act
            var result = await _adminRepo.GetOTDetails(period, companyCode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(PayrollAPI.DataModel.MsgDto));
            result.MsgCode.Should().BeEquivalentTo('E');
            result.Message.Should().BeEquivalentTo("No Data Available");
        }

        #endregion




        #region Mock Data
        #region Pay Code Data

        public static IEnumerable<object[]> PayCodeDataList => new List<object[]>
        {
            // Blank Object
            new object[] { new PayCodeDto { } },
            // No Company Code
            new object[] { new PayCodeDto { payCode = 13, calCode = "_10", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
            // Same PayCode
            new object[] { new PayCodeDto { companyCode = 3000, payCode = 10, calCode = "_10", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
            // No Pay Code
            new object[] { new PayCodeDto { companyCode = 3000, calCode = "_10", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdatePayCodeDataList => new List<object[]>
        {
            new object[] { new PayCodeDto { id = 214, companyCode = 3000, payCode = 111, calCode = "_111", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },

            // No Cal Code
            new object[] { new PayCodeDto { id = 214, companyCode = 3000, payCode = 111, description = "Basic Salary", rate = 0.5m, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdatePayCodeNotFoundDataList => new List<object[]>
        {
            // Blank Object
            new object[] { new PayCodeDto { } },
            // No Company Code
            new object[] { new PayCodeDto { payCode = 999, companyCode = 3000, calCode = "_10", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdatePayCodeErrorDataList => new List<object[]>
        {
            // Cal Code max characters exceeded
            new object[] { new PayCodeDto { id = 214, companyCode = 3000, payCode = 111, calCode = "_111111", description = "Basic Salary", rate = 1, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },

            // payCategory max characters exceeded
            new object[] { new PayCodeDto { id = 214, companyCode = 3000, payCode = 111, calCode = "_111", payCategory = "TEST", description = "Basic Salary", rate = 0.5m, taxationType = "IT", createdBy = "3021ITFI", createdDate = new DateTime() } },
        };

        #endregion

        #region Calculation Data
        public static IEnumerable<object[]> CalculationDataList => new List<object[]>
        {
            // Blank Object
            new object[] { new CalDto { } },
            // No Company Code
            new object[] { new CalDto {
                sequence = 10,
                payCode = 111,
                calCode = "_111",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Test Calculation Data",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
            // Same Cal Code
            new object[] { new CalDto { companyCode = 3000,
                sequence = 11,
                payCode = 112,
                calCode = "EPFEM",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Test Calculation Data",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
            // No Sequence
            new object[] { new CalDto { companyCode = 3000,
                payCode = 112,
                calCode = "ABC",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Test Calculation Data",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdateCalculationDataList => new List<object[]>
        {
            new object[] { new CalDto { id = 12, companyCode = 3000, sequence = 10, payCode = 0,
                calCode = "_111",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Basic Salary Update",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },

            // No Cal Code
            new object[] { new CalDto { id = 12, companyCode = 3000, sequence = 10, payCode = 0,
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Basic Salary",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdateCalculationNotFoundDataList => new List<object[]>
        {
            // Blank Object
            new object[] { new CalDto { } },
            // No Company Code
            new object[] { new CalDto { id = 111, companyCode = 3000, sequence = 10, payCode = 0,
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Basic Salary",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
        };

        public static IEnumerable<object[]> UpdateCalculationErrorDataList => new List<object[]>
        {
            // Cal Code max characters exceeded
            new object[] { new CalDto { id = 12, companyCode = 3000, sequence = 10, payCode = 0,
                calCode = "_11111",
                payCategory = "1",
                calFormula = "Test",
                calDescription = "Basic Salary Error",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },

            // payCategory max characters exceeded
            new object[] { new CalDto { id = 12, companyCode = 3000, sequence = 10, payCode = 0,
                calCode = "_111",
                payCategory = "111",
                calFormula = "Test",
                calDescription = "Basic Salary Error",
                status = true,
                contributor = "E",
                createdBy = "3021ITFI",
                createdDate = new DateTime() } },
        };

        #endregion
        #region

        #endregion

        #endregion


    }
}
