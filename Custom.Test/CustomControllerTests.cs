using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Web.Controllers;
using Custom.BL.Services;
using Custom.BL.Models;
using Web.Models;
using Custom.BL.Enums;

namespace Custom.Test
{
    public class CustomControllerTests
    {
        [Fact]
        public void IndexTest()
        {
            // Arrange
            var mock = new Mock<ICustomService>();
            var model = new CalculateModel();           
            mock.Setup(repo => repo.GetResult(model)).Returns(GetTestCustom(model));
            var controller = new CustomController(mock.Object);
            
            var modelV = new CustomViewModel();

            // Act
            var result = controller.Index(modelV);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            //var modelAssert = Assert.IsAssignableFrom<CalculateModel>(viewResult.Model);
            Assert.Equal(modelV, viewResult?.Model);
        }
        private int GetTestCustom(CalculateModel model)
        {
            var mock = new Mock<ICustomService>();
            model = new CalculateModel
            {
                CarType = CarType.Truck,
                EngineVolume = 2000,
                FuelType = FuelType.Diesel,
                CarWeight = 3000,
                Price = 8000,
                Year = new System.DateTime(2015, 7, 20),
            };

            return 1;
        }
    }
}