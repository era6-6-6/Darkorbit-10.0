﻿using Ow;
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
        public void AddTickNPC(Tick tick)
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
                        actuallTicks = Ticks.ToList();
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
                        actuallTicks = TicksPlayer.ToList();
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
                        actuallTicks = TicksNPC.ToList();
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
                        actuallTicks = TicksMap.ToList();
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
        
        public Task StartTicker()
        {
            Task.Run(async () =>await TickMain());
            Task.Run(async () => await TickPlayers());
            Task.Run(async () => await TickNpcs());
            Task.Run(async () => await TickMap());
            
            return Task.CompletedTask;

        }
    }
}
