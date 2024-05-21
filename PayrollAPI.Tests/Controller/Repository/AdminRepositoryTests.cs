using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Models;
using PayrollAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                calCode = "_10",
                description = "Basic Salary",
                payCode = 111,
                rate = 1,
                isTaxableGross = true,
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
        public async void AdminRepositoryTests_Get_PayCodesByIdNullOK(int payCode)
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

        #endregion



        public static IEnumerable<object[]> PayCodeDataList => new List<object[]>
        {
            // Blank Object
            new object[] { new PayCodeDto { } },
            // No Company Code
            new object[] { new PayCodeDto { payCode = 13, calCode = "_10", description = "Basic Salary", rate = 1, isTaxableGross = true, createdBy = "3021ITFI", createdDate = new DateTime() } },
            // Same PayCode
            new object[] { new PayCodeDto { companyCode = 3000, payCode = 10, calCode = "_10", description = "Basic Salary", rate = 1, isTaxableGross = true, createdBy = "3021ITFI", createdDate = new DateTime() } },
            // No Pay Code
            new object[] { new PayCodeDto { companyCode = 3000, calCode = "_10", description = "Basic Salary", rate = 1, isTaxableGross = true, createdBy = "3021ITFI", createdDate = new DateTime() } },
        };
    }
}
