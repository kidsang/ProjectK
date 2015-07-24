using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class SpawnManager
    {
        public int WaveCount { get; private set; }
        public int NextWaveIndex { get; private set; }
        public float NextWaveStartTime { get; private set; }

        private float waveIntervalTime;
        private List<SpawnWave> waves = new List<SpawnWave>();
        private SpawnWave currentWave;
        private SpawnWave nextWave;

        public void Load(SpawnSetting setting)
        {
            if (setting == null)
                return;

            waveIntervalTime = setting.WaveIntervalTime;

            Dictionary<int, SpawnWave> waveDict = new Dictionary<int, SpawnWave>();
            foreach (var locationSetting in setting.Locations)
            {
                foreach (var waveSetting in locationSetting.Waves)
                {
                    SpawnWave wave;
                    if (!waveDict.TryGetValue(waveSetting.WaveIndex, out wave))
                    {
                        wave = new SpawnWave();
                        waveDict.Add(waveSetting.WaveIndex, wave);
                    }

                    SpawnEntry entry = new SpawnEntry();
                    entry.PathIndex = locationSetting.PathIndex;
                    entry.DelayTime = waveSetting.DelayTime;
                    entry.IntervalTime = waveSetting.IntervalTime;
                    entry.SpawnTimes = waveSetting.SpawnTimes;
                    entry.SpawnPerTime = waveSetting.SpawnPerTime;
                    entry.TemplateID = waveSetting.TemplateID;
                    wave.Entries.Add(entry);
                }
            }
            waves = waveDict.Values.ToList();

            WaveCount = waves.Count;
        }

        public void Start()
        {
            GotoWave(0);
        }

        public void GotoWave(int waveIndex)
        {
            NextWaveStartTime = 0;
            currentWave = null;

            if (WaveCount <= 0)
                NextWaveIndex = 0;
            else if (waveIndex >= WaveCount)
                NextWaveIndex = WaveCount - 1;

            if (waveIndex < WaveCount)
                nextWave = waves[NextWaveIndex];
        }

        public void Activate(Scene scene, float time)
        {
            if (currentWave != null)
            {
                bool finished = currentWave.Activate(scene, time);
                if (finished)
                    currentWave = null;
            }
            else if (nextWave != null && NextWaveStartTime <= time)
            {
                nextWave.Start(time);
                currentWave = nextWave;
                NextWaveIndex += 1;
                if (NextWaveIndex < WaveCount)
                {
                    nextWave = waves[NextWaveIndex];
                    NextWaveStartTime = time + waveIntervalTime;
                }
                else
                {
                    nextWave = null;
                }
            }
        }

        public bool Finished
        {
            get { return currentWave == null && nextWave == null; }
        }

        public int RemainWaveCount
        {
            get { return WaveCount - NextWaveIndex; }
        }

        class SpawnWave
        {
            public List<SpawnEntry> Entries = new List<SpawnEntry>();

            public void Start(float time)
            {
                foreach (var entry in Entries)
                    entry.Start(time);
            }

            // return finished
            public bool Activate(Scene scene, float time)
            {
                bool finished = true;
                foreach (var entry in Entries)
                    finished = finished && entry.Activate(scene, time);
                return finished;
            }
        }

        class SpawnEntry
        {
            public int PathIndex;
            public float DelayTime;
            public float IntervalTime;
            public int SpawnTimes;
            public int SpawnPerTime;
            public int TemplateID;

            private float nextTime;
            private int remainTimes;

            public void Start(float time)
            {
                nextTime = time + DelayTime;
                remainTimes = SpawnTimes;
            }

            // return finished
            public bool Activate(Scene scene, float time)
            {
                if (remainTimes <= 0)
                    return true;

                if (time < nextTime)
                    return false;

                scene.CreateMonster(PathIndex, TemplateID, SpawnPerTime);
                nextTime = time + IntervalTime;
                remainTimes -= 1;

                return false;
            }
        }
    }
}
