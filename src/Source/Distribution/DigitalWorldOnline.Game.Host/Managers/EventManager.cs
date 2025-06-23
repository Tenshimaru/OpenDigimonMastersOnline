using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.GameHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Game.Managers
{
    public class EventManager
    {
        private readonly AssetsLoader _assets;

        public EventManager(AssetsLoader assets)
        {
            _assets = assets;
        }
        private DateTime roundStartTime;
        private DateTime roundCooldownStartTime;
        private const int roundDurationInSeconds = 180; // TIME PER ROUND
        private const int roundCooldownDuration = 40; // COOLDOWN PER ROUND (mob duration should NEVER BE HIGHER than cooldown of a round)
        private int currentRound = 1;
        private bool roundMessageSent = false;
        private const int restartCooldownDuration = 9000; // Cooldown after all rounds
        private const int summonCooldownDuration = 20; // RESPAWN MOB TIME
        private bool restartMessageSent = false;
        private DateTime lastSummonTime = DateTime.MinValue;
        private bool roundEndMessageSent = false;
        private DateTime roundEndCooldownStartTime = DateTime.MinValue;
        private bool eventStartMessageSent = false;


        public async Task EventServer(MapServer map)
        {
            var mobsList = new Dictionary<int,SummonModel?>
                {
                    { 1, _assets.SummonInfo.FirstOrDefault(x => x.Id == 68) }, //First number is Round,Second is Wave
                    { 2, _assets.SummonInfo.FirstOrDefault(x => x.Id == 68) },
                    { 3, _assets.SummonInfo.FirstOrDefault(x => x.Id == 68) }
                   // { 4, _assets.SummonInfo.FirstOrDefault(x => x.Id == 75) },
                  //  { 5, _assets.SummonInfo.FirstOrDefault(x => x.Id == 75) }
                };
                
            if (roundCooldownStartTime != DateTime.MinValue &&
                (DateTime.Now - roundCooldownStartTime).TotalSeconds < roundCooldownDuration)
            {
                return;
            }

            if (currentRound > mobsList.Count)
            {
                if (!restartMessageSent)
                {
                    map.BroadcastGlobal(new NoticeMessagePacket("Waves have ended running on cooldown next waves in 2hrs 30 mins...").Serialize());
                    restartMessageSent = true;
                    eventStartMessageSent = false;

                }

                roundCooldownStartTime = DateTime.Now;
                await Task.Delay(restartCooldownDuration * 1000);

                currentRound = 1;
                restartMessageSent = false;
                roundEndMessageSent = false;
                roundStartTime = DateTime.MinValue;
                lastSummonTime = DateTime.MinValue;

                roundMessageSent = false;

                return;
            }

            if (currentRound == 1 && !eventStartMessageSent)
            {
                map.BroadcastGlobal(new NoticeMessagePacket("The Event has started!").Serialize());
                eventStartMessageSent = true;

                await Task.Delay(1500);

                map.BroadcastGlobal(new NoticeMessagePacket($"Round {currentRound} has started!").Serialize());
                roundMessageSent = true;
            }

            if (roundStartTime == DateTime.MinValue)
            {
                roundStartTime = DateTime.Now;
            }

            if ((DateTime.Now - roundStartTime).TotalSeconds >= roundDurationInSeconds)
            {
                if (currentRound < mobsList.Count)
                {
                    if (!roundEndMessageSent)
                    {
                        map.BroadcastGlobal(new NoticeMessagePacket($"Round {currentRound} has ended. Next round will start in 40 seconds...").Serialize());
                        roundEndMessageSent = true;
                        roundEndCooldownStartTime = DateTime.Now;
                    }

                    if ((DateTime.Now - roundEndCooldownStartTime).TotalSeconds >= 15)
                    {
                        currentRound++;
                        roundMessageSent = false;
                        roundEndMessageSent = false;
                        roundStartTime = DateTime.Now;
                        lastSummonTime = DateTime.MinValue;
                        roundEndCooldownStartTime = DateTime.MinValue;
                    }
                    return;
                }
                else
                {
                    currentRound++;
                    roundMessageSent = false;
                    roundEndMessageSent = false;
                    roundStartTime = DateTime.Now;
                    lastSummonTime = DateTime.MinValue;
                    roundEndCooldownStartTime = DateTime.MinValue;
                    return;
                }
            }

            if (!roundMessageSent && mobsList.ContainsKey(currentRound))
            {
                if (currentRound == 1)
                {
                    return;
                }
                else
                    map.BroadcastGlobal(new NoticeMessagePacket($"Round {currentRound} has started!").Serialize());
                roundMessageSent = true;
            }

            if (mobsList.TryGetValue(currentRound,out var mobs) && mobs != null)
            {
                if (lastSummonTime == DateTime.MinValue || (DateTime.Now - lastSummonTime).TotalSeconds >= summonCooldownDuration)
                {
                    await SummonMobs(mobs,map);
                    lastSummonTime = DateTime.Now;
                }
            }
        }

        private async Task SummonMobs(SummonModel summonInfo, MapServer maps)
        {
            foreach (var mobToAdd in summonInfo.SummonedMobs)
            {
                var mob = (SummonMobModel)mobToAdd.Clone();
                var matchingMaps = maps.Maps.Where(x => x.MapId == mob.Location.MapId).ToList();

                foreach (var map in matchingMaps)
                {
                    if (map.SummonMobs.Any(existingMob => existingMob.Id == mob.Id))
                    {
                        continue;
                    }

                    mob.TamersViewing.Clear();
                    mob.Reset();
                    mob.SetRespawn();
                    mob.SetId(mob.Id);
                    mob.SetLocation(mob.Location.MapId,mob.Location.X,mob.Location.Y);
                    mob.SetDuration();
                    maps.AddSummonMobs(mob.Location.MapId,mob);
                }
            }
        }


    }
}
