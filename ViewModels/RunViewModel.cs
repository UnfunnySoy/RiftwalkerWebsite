using ProjectWebsite.Models;

namespace RiftwalkerWebsite.ViewModels
{
    public class RunViewModel
    {
        public string Username {  get; set; }
        public int Seed { get; set; }
        public int Score { get; set; }
        public TimeSpan Duration { get; set; }
        public RunViewModel(RunModel run)
        {
            Username = "placeholder";
            Seed = run.Seed;
            Score = run.Score;
            Duration = run.EndTime - run.StartTime;
        }
    }
}
