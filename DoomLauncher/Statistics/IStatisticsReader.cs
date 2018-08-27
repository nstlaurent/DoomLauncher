using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public delegate void NewStatisticsEventHandler(object sender, NewStatisticsEventArgs e);

    public interface IStatisticsReader
    {
        event NewStatisticsEventHandler NewStastics;
        void Start();
        void Stop();
        string[] Errors { get; }
        string LaunchParameter { get; }
        bool ReadOnClose { get; }
        void ReadNow();
        IGameFile GameFile { get; set; }
    }
}
