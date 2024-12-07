using System;
using UnityEngine;
using UnityEngine.Audio;

namespace AutoMenu
{
    [Serializable]
    public class AudioParameter
    {
        public string Label;
        public AudioMixer Mixer;
        public string ParameterName;
    }
}
