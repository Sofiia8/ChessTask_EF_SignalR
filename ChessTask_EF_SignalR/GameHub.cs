using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace ChessTask_EF_SignalR
{
    public class GameHub : Hub
    {
        private readonly IPlayState _playState;

        public GameHub(IPlayState playState)
        {
            _playState = playState;
        }
        public void ChangeStateGo()
        {
            _playState.ChangeStateGo();

        }
        public void ChangeStatePause()
        {
            _playState.ChangeStatePause();

        }
        public void ChangeStateClose()
        {
             _playState.ChangeStateClose();
        }

        public interface IPlayState
        {
            static public event Func<string, Task> EventChangeState;
            public string State { get; set; }
            public CancellationTokenSource cts { get; set; }
            public void ChangeStateBegin();
            public void ChangeStatePause();
            public void ChangeStateGo();
            public void ChangeStateClose();
        }
        public class PlayState : IPlayState
        {
            static public event Func<string, Task> EventChangeState;
            private string _state;
            public string State
            {
                get
                {
                    return _state;
                }
                set
                {
                    _state = value; EventChangeState?.Invoke(_state);
                }
            }
            public CancellationTokenSource cts { get; set; }

            public PlayState()
            {
                ChangeStateBegin();
            }

            public void ChangeStateBegin()
            {
                cts?.Dispose();
                State = "begin";
                cts = new CancellationTokenSource();
            }
            public void ChangeStateGo()
            {
                if (State == "go")
                {
                    State = "pause";
                    State = "go";
                }
                else
                    State = "go";
            }
            public void ChangeStatePause()
            {
                if (State == "go" || State == "continue")
                    State = "pause";
                else if (State == "pause")
                    State = "continue";
            }
            public void ChangeStateClose()
            {
                cts.Cancel();
                EventChangeState = null;
            }

        }
    }

}
