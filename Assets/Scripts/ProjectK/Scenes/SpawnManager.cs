using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class SpawnManager
    {
        private Scene scene;

        public int WaveCount { get; private set; }
        public int CurrentWaveIndex { get; private set; }
        private float waveStartTime;

        public void Init(Scene scene)
        {
            this.scene = scene;
        }

        public void Load(SpawnSetting[] settings)
        {
            //Time.t
        }

        public void Start()
        {
            GotoWave(0);
        }

        public void GotoWave(int waveIndex)
        {
            if (waveIndex >= WaveCount)
                waveIndex = WaveCount - 1;

            CurrentWaveIndex = Mathf.Min(waveIndex, WaveCount - 1);
            CurrentWaveIndex = Mathf.Max(CurrentWaveIndex, 0);
            waveStartTime = -1;
        }

        public void Activate(float time)
        {

        }
    }

    public class SpawnEntry
    {
        public int X;
        public int Y;
        public int WaveIndex;
    }
}
