using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using FlightManagement.Controllers;
using FlightManagement.Services;
using FlightManagement.Repositories;
using FlightManagement.Models;
using FlightManagement.ViewModels;

namespace Tests.Controllers
{
    public class FlightsControllerTests
    {
        private FlightsController CreateController(
            Mock<IFlightService>? flightServiceMock = null,
            Mock<IAirportRepository>? airportRepoMock = null)
        {
            var fMock = flightServiceMock ?? new Mock<IFlightService>();
            var aMock = airportRepoMock ?? new Mock<IAirportRepository>();

            var controller = new FlightsController(fMock.Object, aMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            return controller;
        }

        [Fact]
        public async Task Index_Returns_View_With_Flights()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "F1" },
                new Flight { Id = 2, FlightNumber = "F2" }
            }.AsEnumerable();

            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetAllFlightsAsync()).ReturnsAsync(flights);

            var controller = CreateController(fMock);

            // Act
            var result = await controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(flights, view.Model);
        }

        [Fact]
        public async Task Details_NullId_Returns_NotFound()
        {
            var controller = CreateController();
            var result = await controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Found_Returns_View_With_Flight()
        {
            var flight = new Flight { Id = 5, FlightNumber = "ABC" };
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetFlightByIdAsync(5)).ReturnsAsync(flight);

            var controller = CreateController(fMock);
            var result = await controller.Details(5);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(flight, view.Model);
        }

        [Fact]
        public async Task Create_Get_Returns_ViewModel_With_Defaults_And_Airports()
        {
            var airports = new List<Airport>
            {
                new Airport { Id = 1, IataCode = "AAA", Name = "A", City = "CityA", Country = "CountryA" },
                new Airport { Id = 2, IataCode = "BBB", Name = "B", City = "CityB", Country = "CountryB" }
            };

            var aMock = new Mock<IAirportRepository>();
            aMock.Setup(a => a.GetAllAsync()).ReturnsAsync(airports);

            var controller = CreateController(airportRepoMock: aMock);
            var result = await controller.Create();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlightCreateViewModel>(view.Model);

            Assert.NotNull(model.DepartureAirports);
            Assert.NotNull(model.DestinationAirports);
            Assert.InRange(model.DepartureDate, DateTime.Now, DateTime.Now.AddHours(2));
            Assert.Equal(3.5m, model.FuelConsumptionPerKm);
            Assert.Equal(500m, model.TakeoffFuel);
        }

        [Fact]
        public async Task Create_Post_InvalidModelState_Returns_View_With_PopulatedDropdowns()
        {
            var airports = new List<Airport> { new Airport { Id = 1, IataCode = "AAA", Name = "A", City = "CityA", Country = "CountryA" } };
            var aMock = new Mock<IAirportRepository>();
            aMock.Setup(a => a.GetAllAsync()).ReturnsAsync(airports);

            var controller = CreateController(airportRepoMock: aMock);
            controller.ModelState.AddModelError("FlightNumber", "Required");

            var vm = new FlightCreateViewModel();

            var result = await controller.Create(vm);

            var view = Assert.IsType<ViewResult>(result);
            var returnedVm = Assert.IsType<FlightCreateViewModel>(view.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(returnedVm.DepartureAirports);
            Assert.NotNull(returnedVm.DestinationAirports);
        }

        [Fact]
        public async Task Create_Post_OnSuccess_CreatesAndRedirects()
        {
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.CreateFlightAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()))
                .ReturnsAsync((string flightNumber, int departureAirportId, int destinationAirportId, DateTime departureDate, decimal fuelConsumptionPerKm, decimal takeoffFuel) =>
                    new Flight
                    {
                        Id = 10,
                        FlightNumber = flightNumber,
                        DepartureAirportId = departureAirportId,
                        DestinationAirportId = destinationAirportId,
                        DepartureDate = departureDate,
                        FuelConsumptionPerKm = fuelConsumptionPerKm,
                        TakeoffFuel = takeoffFuel
                    });

            var aMock = new Mock<IAirportRepository>();
            aMock.Setup(a => a.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Airport>());

            var controller = CreateController(fMock, aMock);

            var vm = new FlightCreateViewModel
            {
                FlightNumber = "F123",
                DepartureAirportId = 1,
                DestinationAirportId = 2,
                DepartureDate = DateTime.Now.AddHours(1),
                FuelConsumptionPerKm = 2,
                TakeoffFuel = 100
            };

            var result = await controller.Create(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("SuccessMessage"));
            fMock.Verify(s => s.CreateFlightAsync("F123",1,2,It.IsAny<DateTime>(),2,100), Times.Once);
        }

        [Fact]
        public async Task Edit_Get_NullId_Returns_NotFound()
        {
            var controller = CreateController();
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_NotFound_Returns_NotFound()
        {
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync((Flight?)null);

            var controller = CreateController(fMock);
            var result = await controller.Edit(7);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_Found_Returns_ViewModel_With_Dropdowns()
        {
            var flight = new Flight
            {
                Id = 3,
                FlightNumber = "F3",
                DepartureAirportId = 1,
                DestinationAirportId = 2,
                DepartureDate = DateTime.Now,
                FuelConsumptionPerKm = 2,
                TakeoffFuel = 50,
                CalculatedDistance = 100,
                RequiredFuel = 250
            };

            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetFlightByIdAsync(3)).ReturnsAsync(flight);

            var aMock = new Mock<IAirportRepository>();
            aMock.Setup(a => a.GetAllAsync()).ReturnsAsync(new[] { new Airport { Id = 1, IataCode = "A", Name = "AirportA", City = "CityA", Country = "CountryA" } });

            var controller = CreateController(fMock, aMock);
            var result = await controller.Edit(3);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlightEditViewModel>(view.Model);
            Assert.Equal(3, model.Id);
            Assert.NotNull(model.DepartureAirports);
            Assert.NotNull(model.DestinationAirports);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_Returns_NotFound()
        {
            var controller = CreateController();
            var vm = new FlightEditViewModel { Id = 2 };
            var result = await controller.Edit(1, vm);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModelState_Returns_View_With_PopulatedDropdowns()
        {
            var aMock = new Mock<IAirportRepository>();
            aMock.Setup(a => a.GetAllAsync()).ReturnsAsync(new[] { new Airport { Id = 1, IataCode = "A", Name = "AirportA", City = "CityA", Country = "CountryA" } });

            var controller = CreateController(airportRepoMock: aMock);
            controller.ModelState.AddModelError("FlightNumber", "Required");

            var vm = new FlightEditViewModel { Id = 1 };
            var result = await controller.Edit(1, vm);

            var view = Assert.IsType<ViewResult>(result);
            var returnedVm = Assert.IsType<FlightEditViewModel>(view.Model);
            Assert.NotNull(returnedVm.DepartureAirports);
            Assert.NotNull(returnedVm.DestinationAirports);
        }

        [Fact]
        public async Task Delete_Get_NullId_Returns_NotFound()
        {
            var controller = CreateController();
            var result = await controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_NotFound_Returns_NotFound()
        {
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync((Flight?)null);

            var controller = CreateController(fMock);
            var result = await controller.Delete(10);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_Found_Returns_View_With_Flight()
        {
            var flight = new Flight { Id = 11, FlightNumber = "DEL" };
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetFlightByIdAsync(11)).ReturnsAsync(flight);

            var controller = CreateController(fMock);
            var result = await controller.Delete(11);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(flight, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_OnSuccess_DeletesAndRedirects()
        {
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.DeleteFlightAsync(12)).Returns(Task.CompletedTask);

            var controller = CreateController(fMock);

            var result = await controller.DeleteConfirmed(12);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("SuccessMessage"));
            fMock.Verify(s => s.DeleteFlightAsync(12), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_OnException_Sets_ErrorMessage_And_Redirects()
        {
            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.DeleteFlightAsync(It.IsAny<int>())).ThrowsAsync(new Exception("del fail"));

            var controller = CreateController(fMock);

            var result = await controller.DeleteConfirmed(99);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("ErrorMessage"));
        }

        [Fact]
        public async Task Report_Returns_Calculated_Summary()
        {
            var airportA = new Airport { Id = 1, IataCode = "AAA", Name = "A", City = "CityA", Country = "CountryA" };
            var airportB = new Airport { Id = 2, IataCode = "BBB", Name = "B", City = "CityB", Country = "CountryB" };

            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "F1", DepartureAirport = airportA, DestinationAirport = airportB, DepartureDate = DateTime.Today, CalculatedDistance = 100, RequiredFuel = 200 },
                new Flight { Id = 2, FlightNumber = "F2", DepartureAirport = airportB, DestinationAirport = airportA, DepartureDate = DateTime.Today, CalculatedDistance = 200, RequiredFuel = 400 }
            };

            var fMock = new Mock<IFlightService>();
            fMock.Setup(s => s.GetAllFlightsAsync()).ReturnsAsync(flights);

            var controller = CreateController(fMock);
            var result = await controller.Report();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlightReportViewModel>(view.Model);

            Assert.Equal(2, model.Summary.TotalFlights);
            Assert.Equal(300, model.Summary.TotalDistance);
            Assert.Equal(600, model.Summary.TotalFuelRequired);
            Assert.Equal(150, model.Summary.AverageDistance);
            Assert.Equal(300, model.Summary.AverageFuelPerFlight);

            Assert.Contains(model.Flights, i => i.DepartureAirport.Contains("AAA") && i.DestinationAirport.Contains("BBB"));
        }
    }
}