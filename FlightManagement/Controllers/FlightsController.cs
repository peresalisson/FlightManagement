using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightManagement.Models;
using FlightManagement.Services;
using FlightManagement.Repositories;
using FlightManagement.ViewModels;

namespace FlightManagement.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IAirportRepository _airportRepository;

        public FlightsController(IFlightService flightService, IAirportRepository airportRepository)
        {
            _flightService = flightService;
            _airportRepository = airportRepository;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return View(flights);
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var flight = await _flightService.GetFlightByIdAsync(id.Value);
            if (flight == null)
                return NotFound();

            return View(flight);
        }

        // GET: Flights/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new FlightCreateViewModel
            {
                DepartureDate = DateTime.Now.AddHours(1),
                FuelConsumptionPerKm = 3.5m,
                TakeoffFuel = 500m
            };

            await PopulateAirportDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightCreateViewModel viewModel)
        {
            // Validate ModelState
            if (!ModelState.IsValid)
            {
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }

            try
            {
                await _flightService.CreateFlightAsync(
                    viewModel.FlightNumber,
                    viewModel.DepartureAirportId,
                    viewModel.DestinationAirportId,
                    viewModel.DepartureDate,
                    viewModel.FuelConsumptionPerKm,
                    viewModel.TakeoffFuel
                );

                TempData["SuccessMessage"] = "Flight created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating flight: {ex.Message}");
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var flight = await _flightService.GetFlightByIdAsync(id.Value);
            if (flight == null)
                return NotFound();

            var viewModel = new FlightEditViewModel
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                DepartureAirportId = flight.DepartureAirportId,
                DestinationAirportId = flight.DestinationAirportId,
                DepartureDate = flight.DepartureDate,
                FuelConsumptionPerKm = flight.FuelConsumptionPerKm,
                TakeoffFuel = flight.TakeoffFuel,
                CalculatedDistance = flight.CalculatedDistance,
                RequiredFuel = flight.RequiredFuel
            };

            await PopulateAirportDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }

            try
            {
                await _flightService.UpdateFlightAsync(
                    id,
                    viewModel.FlightNumber,
                    viewModel.DepartureAirportId,
                    viewModel.DestinationAirportId,
                    viewModel.DepartureDate,
                    viewModel.FuelConsumptionPerKm,
                    viewModel.TakeoffFuel
                );

                TempData["SuccessMessage"] = "Flight updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating flight: {ex.Message}");
                await PopulateAirportDropdowns(viewModel);
                return View(viewModel);
            }
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var flight = await _flightService.GetFlightByIdAsync(id.Value);
            if (flight == null)
                return NotFound();

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _flightService.DeleteFlightAsync(id);
                TempData["SuccessMessage"] = "Flight deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting flight: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Flights/Report
        public async Task<IActionResult> Report()
        {
            var flights = await _flightService.GetAllFlightsAsync();

            var reportItems = flights.Select(f => new FlightReportItem
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                DepartureAirport = $"{f.DepartureAirport.IataCode} - {f.DepartureAirport.Name}",
                DestinationAirport = $"{f.DestinationAirport.IataCode} - {f.DestinationAirport.Name}",
                DepartureDate = f.DepartureDate,
                CalculatedDistance = f.CalculatedDistance,
                FuelConsumptionPerKm = f.FuelConsumptionPerKm,
                TakeoffFuel = f.TakeoffFuel,
                RequiredFuel = f.RequiredFuel
            }).ToList();

            var summary = new FlightReportSummary
            {
                TotalFlights = reportItems.Count,
                TotalDistance = reportItems.Sum(f => f.CalculatedDistance ?? 0),
                TotalFuelRequired = reportItems.Sum(f => f.RequiredFuel ?? 0),
                AverageDistance = reportItems.Any() ? reportItems.Average(f => f.CalculatedDistance ?? 0) : 0,
                AverageFuelPerFlight = reportItems.Any() ? reportItems.Average(f => f.RequiredFuel ?? 0) : 0
            };

            var viewModel = new FlightReportViewModel
            {
                Flights = reportItems,
                Summary = summary
            };

            return View(viewModel);
        }

        private async Task PopulateAirportDropdowns(FlightCreateViewModel viewModel)
        {
            var airports = await _airportRepository.GetAllAsync();
            viewModel.DepartureAirports = new SelectList(airports, "Id", "IataCode");
            viewModel.DestinationAirports = new SelectList(airports, "Id", "IataCode");
        }
    }
}