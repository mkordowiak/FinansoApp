using AspNetCoreGeneratedDocument;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using FinansoApp.Models;
using FinansoData.Data;
using FinansoData.Repository.Chart;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinansoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IChartData _chartData;

        public HomeController(ILogger<HomeController> logger, IChartData chartData)
        {
            _logger = logger;
            _chartData = chartData;
        }


        private Chart GenerateVerticalBarChart(string dataLabel, List<string> labels, List<double?> numbers)
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Bar;

            Data data = new Data();


            data.Labels = labels;

            BarDataset dataset = new BarDataset()
            {
                Label = dataLabel,
                Data = numbers,
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2)
                },
                BorderColor = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64)
                },
                BorderWidth = new List<int>() { 1 },
                BarPercentage = 0.5,
                BarThickness = 6,
                MaxBarThickness = 8,
                MinBarLength = 2
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            var options = new Options
            {
                Scales = new Dictionary<string, Scale>()
                {
                    { "y", new CartesianLinearScale()
                        {
                            BeginAtZero = true
                        }
                    },
                    { "x", new Scale()
                        {
                            Grid = new Grid()
                            {
                                Offset = true
                            }
                        }
                    },
                }
            };

            chart.Options = options;

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {
                    PaddingObject = new PaddingObject
                    {
                        Left = 10,
                        Right = 12
                    }
                }
            };

            return chart;
        }



        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var incomeData = await _chartData.GetIncomesInCategories(1);
            var expenseData = await _chartData.GetExpensesInCategories(1);

            if (incomeData.IsSuccess == false
                && expenseData.IsSuccess == false)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }


            Chart expensesVerticalChart = GenerateVerticalBarChart("Expenses", expenseData.Value.Select(x => x.Item1).ToList(), expenseData.Value.Select(x => (double?)x.Item2).ToList());
            Chart incomesVerticalChart = GenerateVerticalBarChart("Incomes", incomeData.Value.Select(x => x.Item1).ToList(), incomeData.Value.Select(x => (double?)x.Item2).ToList());
            ViewData["ExpenseVerticalChart"] = expensesVerticalChart;
            ViewData["IncomeVerticalChart"] = incomesVerticalChart;

            return View();
        }

        /// <summary>
        /// Privacy page
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}