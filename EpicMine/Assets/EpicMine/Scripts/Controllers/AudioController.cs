using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class AudioController
    {
        private const int VillageMusicTrackNumber = 1;

        private const int MineMusicTrackNumber = 2;

        private const int MineBossMusicTrackNumber = 3;

        private int _birdsId;

        private int _windId;


        public AudioController()
        {
            AudioManager.Instance.CreateSoundPool(App.Instance.ReferencesTables.Sounds.Click);
            AudioManager.Instance.CreateSoundPool(App.Instance.ReferencesTables.Sounds.OpenWindow);
            AudioManager.Instance.CreateSoundPool(App.Instance.ReferencesTables.Sounds.CloseWindow);

            SceneManager.Instance.OnSceneChange += OnSceneChange;
            EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        public void RefreshSceneSounds()
        {
            OnSceneChange(SceneManager.Instance.CurrentScene, SceneManager.Instance.CurrentScene);
        }


        private void OnSceneChange(string from, string to)
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (from == ScenesNames.Mine)
            {
                if (selectedMine != null && selectedMine.IsLast)
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.MineBossHealing, fadeTime: 0.5f);

                if (to != ScenesNames.Mine)
                    DestroyMineSoundsPools();
            }

            switch (to)
            {
                case ScenesNames.Village:
                {
                    _windId = 0;
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Wind, fadeTime: 0.5f);
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Flame, fadeTime: 0.5f);

                    AudioManager.Instance.PauseMusic(true, MineMusicTrackNumber, fadeTime: 0.5f);
                    AudioManager.Instance.PauseMusic(true, MineBossMusicTrackNumber, fadeTime: 0.5f);
                    AudioManager.Instance.PlayOrContinueMusic(App.Instance.ReferencesTables.Music.Village, track: VillageMusicTrackNumber, loop: true, fadeTime: 0.5f);

                    if (_birdsId <= 0)
                        _birdsId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Birds, loop: true, fadeTime: 0.5f);

                    break;
                }
                case ScenesNames.Tiers:
                {
                    _birdsId = 0;
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Birds, fadeTime: 0.5f);
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Flame, fadeTime: 0.5f);
                    AudioManager.Instance.PauseMusic(true, MineMusicTrackNumber, fadeTime: 0.5f);
                    AudioManager.Instance.PauseMusic(true, MineBossMusicTrackNumber, fadeTime: 0.5f);
                    AudioManager.Instance.PlayOrContinueMusic(App.Instance.ReferencesTables.Music.Village, track: VillageMusicTrackNumber, loop: true, fadeTime: 0.5f);

                    if (_windId <= 0)
                        _windId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Wind, autoPausable: true, loop: true, fadeTime: 0.5f);

                    break;
                }
                case ScenesNames.Mine:
                {
                    if (from != ScenesNames.Mine)
                        CreateMineSoundsPools();

                    AudioManager.Instance.PauseMusic(true, VillageMusicTrackNumber, fadeTime: 0.5f);

                    if (_windId <= 0)
                        _windId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Wind, autoPausable: true, loop: true, fadeTime: 0.5f);

                 //   if (_flameId <= 0)
                 //       _flameId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Flame, autoPausable: true, loop: true, fadeTime: 0.5f);

                    if (selectedMine != null && selectedMine.IsLast)
                    {
                        AudioManager.Instance.PauseMusic(true, MineMusicTrackNumber, fadeTime: 0.5f);
                        AudioManager.Instance.PlayOrContinueMusic(App.Instance.ReferencesTables.Music.MineBoss, track: MineBossMusicTrackNumber, loop: true, fadeTime: 0.5f);
                    }
                    else
                    {
                        AudioManager.Instance.PauseMusic(true, MineBossMusicTrackNumber, fadeTime: 0.5f);
                        AudioManager.Instance.PlayOrContinueMusic(App.Instance.ReferencesTables.Music.Mine, track: MineMusicTrackNumber, loop: true, fadeTime: 0.5f);
                    }
                    break;
                }
                case ScenesNames.PvpArena:
                {
                    if (from != ScenesNames.Mine)
                        CreateMineSoundsPools();

                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Birds, fadeTime: 0.5f);
                    AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.Flame, fadeTime: 0.5f);


                    AudioManager.Instance.PauseMusic(true, VillageMusicTrackNumber, fadeTime: 0.5f);

                    if (_windId <= 0)
                        _windId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Wind,
                            autoPausable: true, loop: true, fadeTime: 0.5f);

                 //   if (_flameId <= 0)
                  //      _flameId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Flame,
                   //         autoPausable: true, loop: true, fadeTime: 0.5f);


                    AudioManager.Instance.PauseMusic(true, MineBossMusicTrackNumber, fadeTime: 0.5f);
                    AudioManager.Instance.PlayOrContinueMusic(App.Instance.ReferencesTables.Music.Mine,
                        track: MineMusicTrackNumber, loop: true, fadeTime: 0.5f);
                    break;
                }
            }
        }

        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (_birdsId <= 0)
                return;

            if (eventData.Window is WindowWorkshop || eventData.Window is WindowShop || eventData.Window is WindowBlacksmith || eventData.Window is WindowTorchesMerchant)
                AudioManager.Instance.SetSoundVolume(_birdsId, 0.4f);
        }

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (_birdsId <= 0)
                return;

            if (eventData.Window is WindowWorkshop || eventData.Window is WindowShop || eventData.Window is WindowBlacksmith || eventData.Window is WindowTorchesMerchant)
                AudioManager.Instance.SetSoundVolume(_birdsId, 1f);
        }

        private void CreateMineSoundsPools()
        {
            foreach (var pickaxeMissSound in App.Instance.ReferencesTables.Sounds.PickaxeMisses)
                AudioManager.Instance.CreateSoundPool(pickaxeMissSound);

            foreach (var pickaxeHitSound in App.Instance.ReferencesTables.Sounds.PickaxeOuterHits)
                AudioManager.Instance.CreateSoundPool(pickaxeHitSound);

            foreach (var increaseDamageSounds in App.Instance.ReferencesTables.Sounds.IncreaseDamages)
                AudioManager.Instance.CreateSoundPool(increaseDamageSounds);

            foreach (var instantDamageSounds in App.Instance.ReferencesTables.Sounds.InstantDamages)
                AudioManager.Instance.CreateSoundPool(instantDamageSounds);

            foreach (var tickingDamageSounds in App.Instance.ReferencesTables.Sounds.TickingDamages)
                AudioManager.Instance.CreateSoundPool(tickingDamageSounds);
        }

        private void DestroyMineSoundsPools()
        {
            foreach (var pickaxeMissSound in App.Instance.ReferencesTables.Sounds.PickaxeMisses)
                AudioManager.Instance.DestroySoundPool(pickaxeMissSound);

            foreach (var pickaxeHitSound in App.Instance.ReferencesTables.Sounds.PickaxeOuterHits)
                AudioManager.Instance.DestroySoundPool(pickaxeHitSound);

            foreach (var increaseDamageSounds in App.Instance.ReferencesTables.Sounds.IncreaseDamages)
                AudioManager.Instance.DestroySoundPool(increaseDamageSounds);

            foreach (var instantDamageSounds in App.Instance.ReferencesTables.Sounds.InstantDamages)
                AudioManager.Instance.DestroySoundPool(instantDamageSounds);

            foreach (var tickingDamageSounds in App.Instance.ReferencesTables.Sounds.TickingDamages)
                AudioManager.Instance.DestroySoundPool(tickingDamageSounds);
        }
    }
}