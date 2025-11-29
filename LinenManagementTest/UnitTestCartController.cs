using FluentAssertions;
using LinenManagement.Controllers;
using LinenManagement.Models;
using LinenManagement.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LinenManagementTest
{
    public class UnitTestCartController
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly CartController _controller;

        public UnitTestCartController()
        {
            _mockCartService = new Mock<ICartService>();
            _controller = new CartController(_mockCartService.Object);
        }

        #region Get By Id

        [Fact]
        public async Task GetCartLogById_Return_OkResult()
        {
            var expectedResult = new CartLogDTO
            {
                CartLogId = 8,
                ReceiptNumber = "1234ABC",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 1,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            };

            _mockCartService.Setup(s => s.GetCartLog(It.IsAny<int>()))
                    .ReturnsAsync(expectedResult);

            // Act
            var data = await _controller.GetCartLog(8);

            // Assert
            data.Should().BeOfType<OkObjectResult>(); 
        }

            [Fact]
        public async Task GetCartLogById_Return_NotFoundResult()
        {
            // Arrange
            _mockCartService.Setup(s => s.GetCartLog(It.IsAny<int>()))
                .ReturnsAsync((CartLogDTO)null);

            // Act
            var data = await _controller.GetCartLog(1);

            // Assert
            data.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCartLogById_MatchResult()
        {
            // Arrange
            var expectedDate = new DateTime(2024, 10, 8, 13, 41, 0);
            var expectedDto = new CartLogDTO
            {
                CartLogId = 2,
                ActualWeight = 51,
                ReportedWeight = 50,
                ReceiptNumber = "123ABC",
                Comments = "Extra blanket received",
                DateWeighed = expectedDate,
                Cart = new object(), 
                Location = new object(),
                Employee = new object(),
                Linen = new List<CartLogLinenDTO>() { new CartLogLinenDTO(), new CartLogLinenDTO(), new CartLogLinenDTO() }
            };

            _mockCartService.Setup(s => s.GetCartLog(2)).ReturnsAsync(expectedDto);

            // Act
            var data = await _controller.GetCartLog(2);

            // Assert
            var okResult = data.Should().BeOfType<OkObjectResult>()
                            .Subject.Value.As<CartLogDTO>();

            Assert.Equal(51, okResult.ActualWeight);
            Assert.Equal(50, okResult.ReportedWeight);
            Assert.Equal("123ABC", okResult.ReceiptNumber);
            Assert.Equal("Extra blanket received", okResult.Comments);
            Assert.Equal(expectedDate, okResult.DateWeighed);
            Assert.Equal(3, okResult.Linen.Count);
        }
        #endregion

        #region Get All

        [Fact]
        public async Task GetAllCartLogs_Return_OkResult()
        {
            // Arrange
            var employeeId = 2;
            var locationId = 3;
            var cartType = "CLEAN";

            var expectedList = new List<CartLogDTO> { new CartLogDTO
            {
                CartLogId = 8,
                ReceiptNumber = "1234ABC",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 3,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            } };

            _mockCartService.Setup(s => s.GetAllCartLogs(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(expectedList);

            // Act
            var employeeFilter = await _controller.GetAllCartLogs(null, employeeId, null);
            var locationFilter = await _controller.GetAllCartLogs(null, null, locationId);
            var typeFilter = await _controller.GetAllCartLogs(cartType, null, null);
            var baseFilter = await _controller.GetAllCartLogs(null, null, null);

            // Assert
            employeeFilter.Should().BeOfType<OkObjectResult>();
            locationFilter.Should().BeOfType<OkObjectResult>();
            typeFilter.Should().BeOfType<OkObjectResult>();
            baseFilter.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAllCartLogs_Return_NotFoundResult()
        {
            // Arrange
            var employeeId = 1;
            var locationId = 4;
            var cartType = "NONE";

            _mockCartService.Setup(s => s.GetAllCartLogs(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync((IEnumerable<CartLogDTO>)null);

            // Act
            var employeeFilter = await _controller.GetAllCartLogs(null, employeeId, null);
            var locationFilter = await _controller.GetAllCartLogs(null, null, locationId);
            var typeFilter = await _controller.GetAllCartLogs(cartType, null, null);
            var baseFilter = await _controller.GetAllCartLogs(null, null, null);

            // Assert
            employeeFilter.Should().BeOfType<NotFoundResult>();
            locationFilter.Should().BeOfType<NotFoundResult>();
            typeFilter.Should().BeOfType<NotFoundResult>();
            baseFilter.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllCartLogs_MatchResult()
        {
            // Arrange
            var employeeId = 2;
            var locationId = 3;
            var cartType = "CLEAN";

            var employeeList = new List<CartLogDTO> { new CartLogDTO
            {
                CartLogId = 10,
                ReceiptNumber = "EMP123",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 1,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            } };
            var locationList = new List<CartLogDTO> { new CartLogDTO
            {
                CartLogId = 20,
                ReceiptNumber = "LOC123",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 3,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            } };
            var typeList = new List<CartLogDTO> { new CartLogDTO
            {
                CartLogId = 30,
                ReceiptNumber = "TYPE123",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 1,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            } };
            var baseList = new List<CartLogDTO> { new CartLogDTO
            {
                CartLogId = 40,
                ReceiptNumber = "BASE123",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                Cart = new Carts
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new Locations
                {
                    LocationId = 1,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new Employee
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = [
                        new CartLogLinenDTO {
                            CartLogDetailId = 1,
                            LinenId = 1,
                            Count = 10,
                            Name = "Bedsheet - Small"
                        },
                        new CartLogLinenDTO {
                                CartLogDetailId = 2,
                                LinenId = 6,
                                Count = 5,
                                Name = "Pillowcase - Large"
                        },
                ]
            } };

            _mockCartService.Setup(s => s.GetAllCartLogs(null, employeeId, null))
                .ReturnsAsync(employeeList);
            _mockCartService.Setup(s => s.GetAllCartLogs(null, null, locationId))
                .ReturnsAsync(locationList);
            _mockCartService.Setup(s => s.GetAllCartLogs(cartType, null, null))
                .ReturnsAsync(typeList);
            _mockCartService.Setup(s => s.GetAllCartLogs(null, null, null))
                .ReturnsAsync(baseList);

            // Act
            var employeeFilter = await _controller.GetAllCartLogs(null, employeeId, null);
            var locationFilter = await _controller.GetAllCartLogs(null, null, locationId);
            var typeFilter = await _controller.GetAllCartLogs(cartType, null, null);
            var baseFilter = await _controller.GetAllCartLogs(null, null, null);

            // Assert
            employeeFilter.Should().BeOfType<OkObjectResult>();
            locationFilter.Should().BeOfType<OkObjectResult>();
            typeFilter.Should().BeOfType<OkObjectResult>();
            baseFilter.Should().BeOfType<OkObjectResult>();

            employeeFilter.As<OkObjectResult>().Value.Should().BeEquivalentTo(employeeList);
            locationFilter.As<OkObjectResult>().Value.Should().BeEquivalentTo(locationList);
            typeFilter.As<OkObjectResult>().Value.Should().BeEquivalentTo(typeList);
            baseFilter.As<OkObjectResult>().Value.Should().BeEquivalentTo(baseList);
        }
        #endregion

        #region Insert CartLog

        [Fact]
        public async void Task_Add_ValidData_Return_OkResult()
        {
            //Arrange
            var cartLog = new CartLog()
            {
                CartLogId = 0,
                ReceiptNumber = "1C2B3A",
                ReportedWeight = 50,
                ActualWeight = 45,
                DateWeighed = DateTime.UtcNow,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            var createdCartLog = new CartLog()
            {
                CartLogId = 101,
                ReceiptNumber = "1C2B3A",
                ReportedWeight = 50,
                ActualWeight = 45,
                DateWeighed = DateTime.UtcNow,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            _mockCartService.Setup(s => s.CreateUpdateCartLog(It.IsAny<CartLog>()))
                .ReturnsAsync(createdCartLog);

            // Act
            var data = await _controller.CreateUpdateCartLog(cartLog);

            // Assert
            data.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            //Arrange
            var cartLog = new CartLog()
            {
                CartLogId = 0,
                ReceiptNumber = "1C2B3A",
                ReportedWeight = 50,
                ActualWeight = 45,
                DateWeighed = DateTime.Parse("1752-12-31T23:59:59"),
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            _mockCartService.Setup(s => s.CreateUpdateCartLog(cartLog))
                .ReturnsAsync((CartLog)null);

            //Act
            var data = await _controller.CreateUpdateCartLog(cartLog);

            //Assert
            data.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async void Task_Add_ValidData_MatchResult()
        {
            //Arrange
            var cartLog = new CartLog()
            {
                CartLogId = 0,
                ReceiptNumber = "1C2B3AXYZ",
                ReportedWeight = 50,
                ActualWeight = 51,
                DateWeighed = DateTime.UtcNow,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            var expectedResult = new CartLog()
            {
                CartLogId = 25,
                ReceiptNumber = "1C2B3AXYZ",
                ReportedWeight = 50,
                ActualWeight = 51,
                DateWeighed = cartLog.DateWeighed,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            _mockCartService.Setup(s => s.CreateUpdateCartLog(cartLog))
                    .ReturnsAsync(expectedResult);

            //Act
            var data = await _controller.CreateUpdateCartLog(cartLog);

            // Assert
            data.Should().BeOfType<OkObjectResult>();

            var okResultValue = data.Should().BeOfType<OkObjectResult>()
                            .Subject.Value.As<CartLog>();

            Assert.Equal(25, okResultValue.CartLogId);
            Assert.Equal(51, okResultValue.ActualWeight);
            Assert.Equal("1C2B3AXYZ", okResultValue.ReceiptNumber);
        }
        #endregion

        #region Update CartLog

        [Fact]
        public async Task Task_Update_ValidData_Return_OkResult()
        {
            //Arrange
            var cartLog = new CartLog()
            {
                CartLogId = 2,
                ReceiptNumber = "123ABC",
                ReportedWeight = 50,
                ActualWeight = 45,
                DateWeighed = DateTime.Parse("2024-10-08T13:41:00"),
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            _mockCartService.Setup(s => s.CreateUpdateCartLog(cartLog))
                        .ReturnsAsync(cartLog);

            //Act
            var updatedCartLog = await _controller.CreateUpdateCartLog(cartLog);

            //Assert
            updatedCartLog.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Task_Update_InvalidData_Return_BadRequest()
        {
            //Arrange
            var cartLog = new CartLog()
            {
                CartLogId = 2,
                ReceiptNumber = "123ABC",
                ReportedWeight = 50,
                ActualWeight = 45,
                DateWeighed = DateTime.Parse("1752-12-31T23:59:59"),
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            _mockCartService.Setup(s => s.CreateUpdateCartLog(cartLog))
        .                   ReturnsAsync((CartLog)null);

            //Act
            var updatedCartLog = await _controller.CreateUpdateCartLog(cartLog);

            //Assert
            updatedCartLog.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Task_Update_ValidData_MatchResult()
        {
            // Arrange
            var originalDate = new DateTime(2024, 10, 8, 13, 41, 0);
            var updatedReceiptNumber = "UPDATED-XYZ";

            var cartLog = new CartLog()
            {
                CartLogId = 5,
                ReceiptNumber = updatedReceiptNumber,
                ReportedWeight = 100,
                ActualWeight = 95,
                DateWeighed = originalDate,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 2
            };

            var expectedResult = cartLog;

            _mockCartService.Setup(s => s.CreateUpdateCartLog(cartLog))
                .ReturnsAsync(expectedResult);

            // Act
            var data = await _controller.CreateUpdateCartLog(cartLog);

            // Assert
            data.Should().BeOfType<OkObjectResult>();

            var okResultValue = data.Should().BeOfType<OkObjectResult>()
                            .Subject.Value.As<CartLog>();

            Assert.Equal(updatedReceiptNumber, okResultValue.ReceiptNumber);
            Assert.Equal(5, okResultValue.CartLogId);
        }

        #endregion

        #region Delete CartLog

        [Fact]
        public async Task DeleteCartLog_Return_OkResult()
        {
            // Arrange
            var cartLogId = 10;
            var employeeId = 2;

            _mockCartService.Setup(s => s.DeleteCartLog(cartLogId, employeeId))
                    .ReturnsAsync(true);

            // Act
            var data = await _controller.DeleteCartLog(cartLogId, employeeId);

            // Assert
            data.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteCartLog_Return_NotFoundResult()
        {
            // Arrange
            var cartLogId = 1;
            var employeeId = 2;

            _mockCartService.Setup(s => s.DeleteCartLog(cartLogId, employeeId))
                    .ReturnsAsync(false);

            // Act
            var data = await _controller.DeleteCartLog(cartLogId, employeeId);

            // Assert
            data.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteCartLog_MatchResult()
        {
            // Arrange
            var cartLogId = 5;
            var employeeId = 99;

            _mockCartService.Setup(s => s.DeleteCartLog(cartLogId, employeeId))
                .ReturnsAsync(true);

            // Act
            var data = await _controller.DeleteCartLog(cartLogId, employeeId);

            // Assert
            data.Should().BeOfType<OkObjectResult>();

            var okResult = data.As<OkObjectResult>();
        }
        #endregion
    }
}