using ChessTask_EF_SignalR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ChessTask_EF_SignalR.GameHub;
using Microsoft.Extensions.Configuration;

namespace ChessTask_EF_SignalR.Controllers
{
    public class HomeController : Controller
    {
        int counter;
        int prev_step;
        double interval;

        private IPlayState _playState;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly object balanceLock = new object();

        private readonly ChessDbContext _context;
        private readonly IConfiguration _config;

        public HomeController(/*IConfiguration configuration,*/ ChessDbContext context, IHubContext<GameHub> hubContext, IPlayState playState)
        {
            _context = context;
            _hubContext = hubContext;
            _playState = playState;

            counter = 0;
            prev_step = 0;
            interval = 0.5;
        }

        public IActionResult Index()
        {
            return View(_context.Steps.ToList());
        }

        public async Task CheckStateGame(string state)
        {
            if (state == "go")
            {
                counter = 0;
                prev_step = 0;
                await Task.Run(() => ProceedGame(state), _playState.cts.Token);
            }
            else if (state == "pause")
            {
                await Task.Delay(TimeSpan.FromSeconds(interval), _playState.cts.Token);
            }
            else if (state == "continue")
            {
                await Task.Run(() => ProceedGame(state), _playState.cts.Token);
            }

        }
        public IActionResult Start()
        {
            return View();
        }
        private void SendMessage(int x, int y, int prev_x, int prev_y, string message)
        {
            _hubContext.Clients.All.SendAsync("Receive", x, y, prev_x, prev_y, message);
        }

        public void ProceedGame(string state)
        {
            lock (balanceLock)
            {

                //using (var context = new ChessDbContext(new DbContextOptionsBuilder<ChessDbContext>()
                //            .UseNpgsql(_config.GetConnectionString("r"))
                //            .Options))
                //{
                    while (!_playState.cts.Token.IsCancellationRequested && counter < _context.Steps.Count() && _playState.State == state )
                    {
                        var item = _context.Steps.Where(step => step.StepNumber == counter).First();
                        var prev_item = _context.Steps.Where(step => step.StepNumber == prev_step).First();
                        SendMessage(item.X, item.Y, prev_item.X, prev_item.Y, $"Ход коня №{counter}: ");/*{Thread.CurrentThread.ManagedThreadId} поток: */
                    
                        prev_step = counter;
                        counter++;
                        if (_playState.State == "pause" || _playState.cts.Token.IsCancellationRequested)
                            return;
                        Thread.Sleep(TimeSpan.FromSeconds(interval));
                    }
               // }
                
            }
        }
        [HttpPost]
        public async Task<IActionResult> Start(int x_start, int y_start, double t)
        {
            _playState.ChangeStateBegin();
            interval = t;
            PlayState.EventChangeState += CheckStateGame;
            await DbInitializer.CorrectStepsNumber(x_start, y_start,_context);
            ViewBag.coord_x = _context.Steps.Where(step => step.StepNumber == counter).First().X;
            ViewBag.coord_y = _context.Steps.Where(step => step.StepNumber == counter).First().Y;
            return View("TestTable");
        }
    }
}
