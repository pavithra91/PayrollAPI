using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Controllers;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using PayrollAPI.Repository;

namespace PayrollAPI.Test
{
    public class AdminControllerTest
    {
        AdminController adminController;
        IAdmin admin;
        public AdminControllerTest()
        {
            
            adminController = new AdminController(admin);
        }
        [Fact]
        public void GetAllTest()
        {
            //Arrange
            //Act
            var result = adminController.GetCalculations();
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;

            Assert.IsType<List<Calculation>>(list.Value);



            var listBooks = list.Value as List<Calculation>;

            Assert.Equal(5, listBooks.Count);
        }
    }
}