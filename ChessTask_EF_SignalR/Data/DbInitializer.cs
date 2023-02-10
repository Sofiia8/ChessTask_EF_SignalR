using ChessTask_EF_SignalR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessTask_EF_SignalR.Data
{
    public class DbInitializer
    {
        public static async Task InitializeDefault (ChessDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Steps.Any())
            {
                return;
            }
        
            await CreateMatrixPath(0, 0, context);
            await context.SaveChangesAsync();
        }

        public static async Task CreateMatrixPath(int y_start, int x_start, ChessDbContext context)
        {
            List<List<List<int>>> Matrix = new List<List<List<int>>>();
            var str = new List<List<int>>
            {
                new List<int> { 1, 2 },
                new List<int> { 1, 3 }, // 0-ая строка
                new List<int> { 1, 0 },
                new List<int> { 2, 2 },
                new List<int> { 2, 3 },
                new List<int> { 2, 4 },
                new List<int> { 1, 4 },
                new List<int> { 2, 6 }
            };                          
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 3, 1 },
                new List<int> { 0, 3 }, // 1-ая строка
                new List<int> { 0, 4 },
                new List<int> { 0, 5 },
                new List<int> { 0, 2 },
                new List<int> { 0, 7 },
                new List<int> { 3, 7 },
                new List<int> { 2, 5 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 0, 1 },
                new List<int> { 0, 0 }, // 2-ая строка
                new List<int> { 3, 4 },
                new List<int> { 3, 5 },
                new List<int> { 1, 6 },
                new List<int> { 0, 6 },
                new List<int> { 4, 7 },
                new List<int> { 1, 5 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 1, 1 },
                new List<int> { 5, 0 }, // 3-ая строка
                new List<int> { 5, 3 },
                new List<int> { 5, 2 },
                new List<int> { 4, 2 },
                new List<int> { 4, 3 },
                new List<int> { 1, 7 },
                new List<int> { 5, 6 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 2, 1 },
                new List<int> { 2, 0 }, // 4-ая строка
                new List<int> { 5, 4 },
                new List<int> { 5, 5 },
                new List<int> { 3, 2 },
                new List<int> { 5, 7 },
                new List<int> { 2, 7 },
                new List<int> { 6, 6 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 7, 1 },
                new List<int> { 3, 0 }, // 5-ая строка
                new List<int> { 4, 4 },
                new List<int> { 4, 5 },
                new List<int> { 3, 3 },
                new List<int> { 3, 6 },
                new List<int> { 7, 7 },
                new List<int> { 7, 6 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 4, 1 },
                new List<int> { 4, 0 }, // 6-ая строка
                new List<int> { 7, 0 },
                new List<int> { 7, 5 },
                new List<int> { 7, 2 },
                new List<int> { 7, 3 },
                new List<int> { 7, 4 },
                new List<int> { 4, 6 } 
            };
            Matrix.Add(str);

            str = new List<List<int>>
            {
                new List<int> { 5, 1 },
                new List<int> { 6, 3 }, // 7-ая строка
                new List<int> { 6, 0 },
                new List<int> { 6, 1 },
                new List<int> { 6, 2 },
                new List<int> { 6, 7 },
                new List<int> { 6, 4 },
                new List<int> { 6, 5 } 
            };
            Matrix.Add(str);

            await context.Steps.AddAsync(new Step
            {
                X = x_start,
                Y = y_start,
                StepNumber = 0
            });

            int x_prev = x_start;
            int y_prev = y_start;
            for(int i=1; i< 64; i++) 
            {

                var x_curr = Matrix[x_prev][y_prev][0];
                var y_curr = Matrix[x_prev][y_prev][1];
                await context.Steps.AddAsync(new Step { X = x_curr,
                                                            Y = y_curr, 
                                                            StepNumber = i });
                x_prev = x_curr;
                y_prev = y_curr;
            }

        }
        public static async Task CorrectStepsNumber(int x_start, int y_start, ChessDbContext context)
        {

            var step_start = context.Steps.ToList().Where(st => st.StepNumber == 0).First();
            if (step_start.X == x_start && step_start.Y == y_start)
                return;
            var steps = context.Steps.ToList();
            foreach (Step s in steps)
            {
                context.Steps.Remove(s);
            }

            await CreateMatrixPath(x_start, y_start, context);
            await context.SaveChangesAsync();

        }
       
    }
}
