using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Gnomic.Audio
{
#if !USE_XACT
    public enum AudioStopOptions
    {
        AsAuthored = 0,
        Immediate = 1,
    }

    public class SoundEffectSettings
    {
        public SoundEffect Sound;

        public float Volume = 1.0f;
        public float Pitch = 0.0f;
        public bool Loop = false;
        public int MaxInstances = 0;

        public List<SoundEffectInstance> ActiveInstances;

        public SoundEffectSettings(SoundEffect sound, float volume, float pitch, bool loop)
            : this(sound, volume, pitch, loop, 0)
        {
        }

        public SoundEffectSettings(SoundEffect sound, float volume, float pitch, bool loop, int maxInstances)
        {
            Sound = sound;
            Volume = volume;
            Pitch = pitch;
            Loop = loop;

            MaxInstances = maxInstances;
            if (MaxInstances > 0)
                ActiveInstances = new List<SoundEffectInstance>(MaxInstances);
        }
    }

    public class Cue
    {
        public string Name;
        public SoundEffectInstance Sound;
        public SoundEffectSettings Settings;

        public bool IsPlaying
        {
            get { return !Sound.IsDisposed && Sound.State == SoundState.Playing; }
        }
        public bool IsPaused
        {
            get { return !Sound.IsDisposed && Sound.State == SoundState.Paused; }
        }
        public bool IsStopped
        {
            get { return Sound.IsDisposed || Sound.State == SoundState.Stopped; }
        }
        public bool IsDisposed
        {
            get { return Sound.IsDisposed; }
        }

        public void Play()
        {
            if (!Sound.IsDisposed)
                Sound.Play();
        }
        public void Pause()
        {
            if (!Sound.IsDisposed)
                Sound.Pause();
        }
        public void Resume()
        {
            if (!Sound.IsDisposed)
                Sound.Resume();
        }
        public void Stop(AudioStopOptions options)
        {
            if (!Sound.IsDisposed)
                Sound.Stop(options == AudioStopOptions.Immediate);
        }
    
        public void Dispose()
        {
            Sound.Dispose();
        }
    }
#endif


    public class AudioManager
    {
        // Audio objects
#if !USE_XACT
        List<Cue> cuePool = new List<Cue>();
        Dictionary<string, SoundEffectSettings> soundEffects = new Dictionary<string, SoundEffectSettings>();
        ContentManager content;

        float soundFXVol = 1.0f;
        float musicVol = 1.0f;
#else
        AudioEngine engine;
        SoundBank currentSoundBank;
        Dictionary<string, WaveBank> waveBanks;
#endif
        List<Cue> activeCues = new List<Cue>(32);

        // Song members
        public List<string> SongList = new List<string>();
        Dictionary<string, Song> songs = new Dictionary<string, Song>();
        Song currentMediaPlayerSong;

        bool gameHasControl = false;
        Cue currentSongCue;
        float currentSongDelayTime = -1f;
        float currentSongDelayTimeTotal = 2.0f;

        int currentSongId;
        public int CurrentSongId
        {
            get { return currentSongId; }
        }

        public AudioManager()
        {
#if !USE_XACT
            for (int i = 0; i < 20; ++i)
                cuePool.Add(new Cue());
#endif
        }

#if !USE_XACT
        public void PreloadSoundEffect(string name, string contentPath, bool loop)
        {
            PreloadSoundEffect(name, contentPath, loop, 1.0f, 0.0f, 0);
        }
        public void PreloadSoundEffect(string name, string contentPath, bool loop, int maxInstances)
        {
            PreloadSoundEffect(name, contentPath, loop, 1.0f, 0.0f, maxInstances);
        }
        public void PreloadSoundEffect(string name, string contentPath, bool loop, float volume, float pitch, int maxInstances)
        {
            soundEffects.Add(
                name, 
                new SoundEffectSettings(
                    content.Load<SoundEffect>(contentPath),
                    volume, pitch, loop, maxInstances));  
        }
#endif

        public void AddMediaPlayerSong(string songName, string songContentPath)
        {
            SongList.Add(songName);
            songs[songName] = content.Load<Song>(songContentPath);
        }

        public void AddSong(string songCueName)
        {
            SongList.Add(songCueName);
        }

        public void Initialise(string settingsPath)
        {
#if USE_XACT
            engine = new AudioEngine(settingsPath);
            waveBanks = new Dictionary<string, WaveBank>();
#endif
        }

        public Cue Play(string name)
        {
            return Play(name, false);
        }

#if !USE_XACT
        Cue FindInactiveSoundEffectCue()
        {
            foreach (Cue c in cuePool)
            {
                if (c.Sound == null || c.IsDisposed || c.Sound.State == SoundState.Stopped)
                {
                    return c;
                }
            }
            return null;
        }
#endif

        Queue<string> toPlay = new Queue<string>(10);
        int playedCount = 0;

        public Cue Play(string name, bool keepCue)
        {
            //return null;
            if (!keepCue && playedCount > 0 && toPlay.Count < 6)
            {
                toPlay.Enqueue(name);
                return null;
            }
            playedCount++;

#if USE_XACT
            Cue c = currentSoundBank.GetCue(name);
            c.Play();
            if (keepCue)
                activeCues.Add(c);
            return c;
#else
            SoundEffectSettings sfxs = null;
            if (soundEffects.TryGetValue(name, out sfxs))
            {
                if (sfxs.MaxInstances > 0)
                {
                    for (int i = sfxs.ActiveInstances.Count - 1; i >= 0; --i)
                    {
                        if (sfxs.ActiveInstances[i].IsDisposed || sfxs.ActiveInstances[i].State == SoundState.Stopped)
                            sfxs.ActiveInstances.RemoveAt(i);
                    }

                    if (sfxs.ActiveInstances.Count > sfxs.MaxInstances)
                        return null;
                }

                if (keepCue || sfxs.Loop)
                {
                    Cue c = FindInactiveSoundEffectCue();
                    if (c != null)
                    {
                        c.Name = name;
                        c.Settings = sfxs;
                        c.Sound = sfxs.Sound.CreateInstance();
                        c.Sound.IsLooped = sfxs.Loop;
                        c.Sound.Volume = sfxs.Volume; // *soundFXVol;
                        c.Sound.Pitch = sfxs.Pitch;
                        c.Sound.Play();

                        activeCues.Add(c);
                        if (sfxs.MaxInstances > 0)
                            sfxs.ActiveInstances.Add(c.Sound);

                        return c;
                    }
                }
                else if (sfxs.MaxInstances > 0)
                {
                    SoundEffectInstance sfi = sfxs.Sound.CreateInstance();
                    sfi.IsLooped = false;
                    sfi.Volume = sfxs.Volume; // *soundFXVol;
                    sfi.Pitch = sfxs.Pitch;
                    sfi.Play();

                    sfxs.ActiveInstances.Add(sfi);
                }
                else
                {
                    sfxs.Sound.Play(sfxs.Volume, sfxs.Pitch, 0.0f);  //* soundFXVol
                }
            }
            return null;
#endif
        }

        public bool IsSongPlaying(int id)
        {
            if (currentMediaPlayerSong != null)
                return currentMediaPlayerSong == songs[SongList[id]];

            if (currentSongCue == null || currentSongCue.IsStopped)
                return false;

            return currentSongCue.Name == SongList[id];
        }

        public Cue GetCurrentSongCue()
        {
            return currentSongCue;
        }
        public Cue PlaySong(int id)
        {
            return PlaySong(id, 0.0f);
        }

        public Cue PlaySong(int id, float delayTime)
        {
            if (SongList.Count > id && id >= 0)
            {
                currentSongId = id;
                return PlaySong(SongList[id], delayTime);
            }

            return null;
        }


        public Cue PlaySong(string name, float delayTime)
        {
            if (currentSongCue != null && currentSongCue.IsPlaying)
                currentSongCue.Stop(AudioStopOptions.AsAuthored);

            
#if !USE_XACT
            currentMediaPlayerSong = songs[name];
            if (delayTime <= 0.0f)
            {
                gameHasControl = MediaPlayer.GameHasControl;
                if (gameHasControl)
                {
                    try
                    {
                        MediaPlayer.Play(currentMediaPlayerSong);
                        MediaPlayer.IsRepeating = true;
                    }
                    catch { }
                }
            }
            else
            {
                currentSongDelayTime = delayTime;
                currentSongDelayTimeTotal = delayTime;
            }
            return null;
#else
            currentSongCue = currentSoundBank.GetCue(name);            
            if (delayTime <= 0.0f)
            {
                currentSongDelayTime = -1.0f;

                currentSongCue.Play();
            }
            else
            {
                currentSongDelayTime = delayTime;
            }

            return currentSongCue;
#endif
        }

        public void SetCurrentSongCue(Cue songCue)
        {
            currentSongCue = songCue;
        }

        public void StopCurrentSong()
        {
            if (currentSongCue != null)
            {
                currentSongCue.Stop(AudioStopOptions.AsAuthored);
                currentSongCue = null;
            }
            else
            {
                gameHasControl = MediaPlayer.GameHasControl;
                if (gameHasControl)
                {
                    try
                    {
                        MediaPlayer.Stop();
                    }
                    catch { }
                    currentMediaPlayerSong = null;
                }
            }
        }

        public void Pause(bool pauseSong)
        {
            foreach (Cue c in activeCues)
            {
                if (!c.IsStopped)
                    c.Pause();
            }

            if (pauseSong)
            {
                if (currentSongCue != null &&
                    !currentSongCue.IsPaused)
                {
                    currentSongCue.Pause();
                }
                gameHasControl = MediaPlayer.GameHasControl;
                if (gameHasControl)
                {
                    try
                    {
                        MediaPlayer.Pause();
                    }
                    catch { }
                }
            }
        }

        public void Resume()
        {
            foreach (Cue c in activeCues)
            {
                c.Resume();
            }

            if (currentSongCue != null &&
                currentSongCue.IsPaused)
            {
                currentSongCue.Resume();
            }
            gameHasControl = MediaPlayer.GameHasControl;
            if (gameHasControl)
            {
                try
                {
                    MediaPlayer.Resume();
                }
                catch { }
            }
        }

        public void Stop(AudioStopOptions options, bool stopSong)
        {
            foreach (Cue c in activeCues)
            {
                c.Stop(options);
            }
            activeCues.Clear();

            if (stopSong)
            {
                if (currentSongCue != null &&
                    !currentSongCue.IsDisposed)
                {
                    currentSongCue.Stop(options);
                }
                gameHasControl = MediaPlayer.GameHasControl;
                if (gameHasControl)
                {
                    try
                    {
                        MediaPlayer.Stop();
                    }
                    catch { }
                    currentMediaPlayerSong = null;
                }
            }
        }

        public void NextSong()
        {
            if (SongList.Count == 0)
                return;

            if (currentSongCue != null && currentSongCue.IsPlaying)
                currentSongCue.Stop(AudioStopOptions.AsAuthored);

            currentSongId = ++currentSongId % SongList.Count;

#if !USE_XACT
            gameHasControl = MediaPlayer.GameHasControl;
            if (gameHasControl)
            {
                currentMediaPlayerSong = songs[SongList[currentSongId]];
                try
                {
                    MediaPlayer.Play(currentMediaPlayerSong);
                    MediaPlayer.IsRepeating = true;
                }
                catch { }
            }
#else
            currentSongCue = currentSoundBank.GetCue(SongList[currentSongId]);
            currentSongCue.Play();
#endif
        }
        public void PrevSong()
        {
            if (SongList.Count == 0)
                return;

            if (currentSongCue != null && currentSongCue.IsPlaying)
                currentSongCue.Stop(AudioStopOptions.AsAuthored);

            currentSongId = (--currentSongId + SongList.Count) % SongList.Count;

#if !USE_XACT
            gameHasControl = MediaPlayer.GameHasControl;
            if (gameHasControl)
            {
                currentMediaPlayerSong = songs[SongList[currentSongId]];
                try
                {
                    MediaPlayer.Play(currentMediaPlayerSong);
                    MediaPlayer.IsRepeating = true;
                }
                catch { }
            }
#else
            currentSongCue = currentSoundBank.GetCue(SongList[currentSongId]);         
            currentSongCue.Play();
#endif

        }

#if USE_XACT
        public void LoadSoundBank(string path)
        {
            currentSoundBank = new SoundBank(engine, path);
        }

        public void LoadWaveBank(string path)
        {
            WaveBank wb = new WaveBank(engine, path);
            waveBanks.Add(path, wb);
        }

        public void LoadWaveBankStreaming(string path)
        {
            WaveBank wb = new WaveBank(engine, path, 0, 64);
            waveBanks.Add(path, wb);
        }
#endif

        public void Update(float dt)
        {
#if USE_XACT
            if (engine == null)
                return;

            engine.Update();
#endif
            playedCount = 0;
            if (toPlay.Count > 0)
            {
                Play(toPlay.Dequeue(), false);
            }

            if (currentSongDelayTime > 0.0f)
            {
                currentSongDelayTime -= dt;
#if !USE_XACT
                if (gameHasControl)
                {
                    // fade out song...
                    float adjustMusicVol = musicVol * currentSongDelayTime / currentSongDelayTimeTotal;
                    try
                    {
                        //if (adjustMusicVol <= 0.0f)
                        //    MediaPlayer.Volume = 0.001f;
                        //else
                            MediaPlayer.Volume = adjustMusicVol * adjustMusicVol;
                    }
                    catch { }
                }
#endif

                if (currentSongDelayTime <= 0.0f)
                {
                    if (currentSongCue != null)
                        currentSongCue.Play();
                    else if (currentMediaPlayerSong != null)
                    {

#if !USE_XACT
                        SetVolumeSongs(musicVol);
#endif
                        gameHasControl = MediaPlayer.GameHasControl;
                        if (gameHasControl)
                        {
                            try
                            {
                                MediaPlayer.Play(currentMediaPlayerSong);
                                MediaPlayer.IsRepeating = true;
                            }
                            catch { }
                        }
                    }
                }
            }
#if !USE_XACT
            else if (gameHasControl && currentSongDelayTime >= 0.0f)
            {
                try
                {
                    // fade out song...
                    float adjustMusicVol = currentSongDelayTime / currentSongDelayTimeTotal;

                    //if (adjustMusicVol <= 0.0f)
                    //    MediaPlayer.Volume = 0.001f;
                    //else
                        MediaPlayer.Volume = adjustMusicVol * adjustMusicVol;
                }
                catch { }
            }
#endif

            for (int i = activeCues.Count - 1; i >= 0; i--)
            {
                if (activeCues[i].IsStopped)
                {
#if !USE_XACT
                    activeCues[i].Dispose();
#endif
                    activeCues.RemoveAt(i);

                }
            }
        }

        public void SetVolumeFX(float amount)
        {
#if !USE_XACT
            soundFXVol = amount;
            SoundEffect.MasterVolume = soundFXVol; // (float)Math.Pow(soundFXVol, 2f);
#else
            engine.SetGlobalVariable("FX_Vol", 65025f * (float)Math.Pow(amount, 0.1f)); //((float)Math.Log(amount, 1000.0) + 1f)); 
#endif
        }
        public void AdjustVolumeFX(float amount)
        {
#if !USE_XACT
            soundFXVol = MathHelper.Clamp(soundFXVol + amount, 0f, 1f);
            SoundEffect.MasterVolume = soundFXVol; // (float)Math.Pow(soundFXVol, 2f);
#else
            float currentLevel = engine.GetGlobalVariable("FX_Vol") / 65025f;
            currentLevel = MathHelper.Clamp(currentLevel + amount, 0f, 1f);
            engine.SetGlobalVariable("FX_Vol", 65025f * currentLevel);
#endif
        }

        public void SetVolumeSongs(float amount)
        {
#if !USE_XACT
            musicVol = amount;
            gameHasControl = MediaPlayer.GameHasControl;
            if (gameHasControl)
            {
                try
                {
                    //if (musicVol == 0.0f)
                    //    MediaPlayer.Volume = 0.001f;
                    //else
                        MediaPlayer.Volume = musicVol * musicVol;
                }
                catch { }
            }
#else
            engine.SetGlobalVariable("Musak_Vol", 65025f * (float)Math.Pow(amount, 0.1f)); //((float)Math.Log(amount, 1000.0) + 1f)); 
#endif
        }

        public void AdjustVolumeSongs(float amount)
        {
#if !USE_XACT
            musicVol = MathHelper.Clamp(musicVol + amount, 0f, 1f);
            gameHasControl = MediaPlayer.GameHasControl;
            if (gameHasControl)
            {
                try
                {
                    //if (musicVol == 0.0f)
                    //    MediaPlayer.Volume = 0.001f;
                    //else
                        MediaPlayer.Volume = musicVol * musicVol;
                }
                catch { }
            }
#else
            float currentLevel = engine.GetGlobalVariable("Musak_Vol") / 65025f;
            currentLevel = MathHelper.Clamp(currentLevel + amount, 0f, 1f);
            engine.SetGlobalVariable("Musak_Vol", 65025f * currentLevel);
#endif
        }

    }
}
