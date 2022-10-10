using Ow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Ticks
{
    public class TickManager
    {
        private List<Tick> Ticks = new();
        private List<Tick> TicksPlayer = new();
        private List<Tick> TicksNPC = new();
        private List<Tick> TicksMap = new();

        public void AddTick(Tick tick)
        {
            lock (Ticks)
            {
                if (!Ticks.Contains(tick))
                    Ticks.Add(tick);
            }
        }
        public void AddTickPlayer(Tick tick)
        {
            lock (TicksPlayer)
            {
                if (!TicksPlayer.Contains(tick))
                    TicksPlayer.Add(tick);
            }
        }
        public void AddNpcTick(Tick tick)
        {
            lock (TicksNPC)
            {
                if (!TicksNPC.Contains(tick))
                    TicksNPC.Add(tick);
            }
        }
    
     
        public void AddTickMap(Tick tick)
        {
            lock (TicksMap)
            {
                if (!TicksMap.Contains(tick))
                    TicksMap.Add(tick);
            }
        }

        public void Remove(Tick tick)
        {
            lock (Ticks)
            {
                if (Ticks.Contains(tick))
                    Ticks.Remove(tick);
            }
            lock (TicksPlayer)
            {
                if (TicksPlayer.Contains(tick))
                    TicksPlayer.Remove(tick);
            }
            lock (TicksNPC)
            {
                if (TicksNPC.Contains(tick))
                    TicksNPC.Remove(tick);
            }
            lock (TicksMap)
            {
                if (TicksMap.Contains(tick))
                    TicksMap.Remove(tick);
            }

        }
        private async Task TickMain()
        {
            try
            {
                while (Program.Running)
                {
                    List<Tick>? actuallTicks = null;
                    lock (Ticks)
                    {
                        actuallTicks = Ticks;
                    }
                    foreach (var tick in actuallTicks)
                    {
                        if (tick != null)
                            tick.Tick();
                    }
                    await Task.Delay(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private async Task TickPlayers()
        {
            try
            {
                while (Program.Running)
                {
                    List<Tick>? actuallTicks = null;
                    lock (TicksPlayer)
                    {
                        actuallTicks = TicksPlayer;
                    }
                    foreach (var tick in actuallTicks)
                    {
                        if (tick != null)
                            tick.Tick();
                    }
                    await Task.Delay(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private async Task TickNpcs()
        {
            try
            {
                while (Program.Running)
                {
                    List<Tick>? actuallTicks = null;
                    lock (TicksNPC)
                    {
                        actuallTicks = TicksNPC;
                    }
                    foreach (var tick in actuallTicks)
                    {
                        if (tick != null)
                            tick.Tick();
                    }
                    await Task.Delay(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private async Task TickMap()
        {
            try
            {
                while (Program.Running)
                {
                    List<Tick>? actuallTicks = null;
                    lock (TicksMap)
                    {
                        actuallTicks = TicksMap;
                    }
                    foreach (var tick in actuallTicks)
                    {
                        if (tick != null)
                            tick.Tick();
                    }
                    await Task.Delay(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    
        public void RemoveTick(Tick tick)
        {
            lock (Ticks)
            {
                if (Ticks.Contains(tick))
                    Ticks.Remove(tick);
            }
            lock (TicksPlayer)
            {
                if (TicksPlayer.Contains(tick))
                    TicksPlayer.Remove(tick);
            }
            lock (TicksNPC)
            {
                if (TicksNPC.Contains(tick))
                    TicksNPC.Remove(tick);
            }
            lock (TicksMap)
            {
                if (TicksMap.Contains(tick))
                    TicksMap.Remove(tick);
            }
        }
        public bool Exists(Tick tick)
        {
            lock (Ticks)
            {
                if (Ticks.Contains(tick))
                    return true;
            }
            lock (TicksPlayer)
            {
                if (TicksPlayer.Contains(tick))
                    return true;
            }
            lock (TicksNPC)
            {
                if (TicksNPC.Contains(tick))
                    return true;
            }
            lock (TicksMap)
            {
                if (TicksMap.Contains(tick))
                    return true;
            }
            return false;
        }


        
        public void StartTicker()
        {
            Task.Run(async () => await TickMain());
            Task.Run(async () => await TickPlayers());
            Task.Run(async () => await TickNpcs());
            Task.Run(async () => await TickMap());
            Console.WriteLine("Ticker started!");
            Console.WriteLine("Actual tick is set to 40ms");
        }
    }
}
