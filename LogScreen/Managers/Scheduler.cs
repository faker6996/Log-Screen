
using System;
using System.Timers;

namespace LogScreen.Managers
{
    public class Scheduler
{
    private Timer timer;
    private Action captureAction;

    public Scheduler(Action action, int interval)
    {
        captureAction = action;
        timer = new Timer(interval * 60 * 1000);
        timer.Elapsed += (sender, e) => captureAction();
    }

    public void Start()
    {
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
    }
}
}








