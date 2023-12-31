using System;
using System.Collections;
using System.Threading.Tasks;
using LiveLarson.GameMode;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using AudioType = LiveLarson.Enums.AudioType;
using Object = UnityEngine.Object;

namespace LiveLarson.SoundSystem
{
    public sealed class Audio
    {
        private const string DEFAULT_AUDIO_CLIP_ON_ERROR = "Audio/Kenney Audio/kenney_digital-audio/Audio/phaseJump2.ogg";

        private static IGameModeService _gameModeService;
        private static readonly IObservable<long> DelayReleaseStream = Observable.Timer(TimeSpan.FromSeconds(1));
        private AudioClip _clip;
        private bool _loop;

        private SoundService _soundService;
        private AsyncOperationHandle<AudioClip> _addressableHandle;
        private bool _autoReleased = true;
        private bool _clipLoading;
        private bool _waitClip;

        public Audio(SoundService soundService, int audioId, AudioType audioType, string clipPath, Vector3 pos, bool loop)
        {
            _soundService = soundService;
            AudioId = audioId;
            AudioType = audioType;
            var audio = new GameObject(ToString());
            audio.transform.SetParent(_soundService.transform);
            audio.transform.position = pos;
            AudioSource = audio.AddComponent<AudioSource>();
            AudioSource.spatialize = true;
            Loop = loop;
            AudioSource.outputAudioMixerGroup =
                _soundService.GetAudioMixer().FindMatchingGroups(AudioType.ToString())[0];
            _clipLoading = true;
            GetAudioClip(clipPath, SetClip);
        }

        private Audio()
        {
        }

        public Action OnStop { get; set; }
        public Action OnEnd { get; set; }

        public int AudioId { get; set; }

        public AudioType AudioType { get; set; }

        public bool IsPlaying { get; set; }

        public AudioSource AudioSource { get; set; }

        public AudioClip Clip
        {
            get => _clip;
            set
            {
                _clip = value;
                if (AudioSource)
                    AudioSource.clip = value;
            }
        }

        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                if (AudioSource)
                    AudioSource.loop = value;
            }
        }

        public bool IsPaused { get; set; }

        public static Audio Create(SoundService service, int audioId, AudioType audioType, AudioClip clip,
            bool autoReleased)
        {
            var instance = new Audio
            {
                _soundService = service,
                AudioId = audioId,
                AudioType = audioType,
                _clip = clip,
                _autoReleased = autoReleased
            };

            var audioGameObject = new GameObject(nameof(Audio));
            audioGameObject.transform.SetParent(service.transform, false);
            instance.AudioSource = audioGameObject.AddComponent<AudioSource>();
            instance.AudioSource.clip = clip;
            instance.AudioSource.outputAudioMixerGroup =
                service.GetAudioMixer().FindMatchingGroups(audioType.ToString())[0];

            return instance;
        }

        private void GetAudioClip(string path, Action<AudioClip> onLoading)
        {
            Addressables.LoadAssetAsync<AudioClip>(path).Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    onLoading?.Invoke(op.Result);
                    _addressableHandle = op;
                }
                else
                {
                    Addressables.LoadAssetAsync<AudioClip>(DEFAULT_AUDIO_CLIP_ON_ERROR).Completed += op2 =>
                    {
                        Addressables.Release(op);
                        if (op2.Status == AsyncOperationStatus.Succeeded)
                        {
                            Debug.LogError(
                                $"Audio error! : {path} 오디오 파일 load 실패. 임시 {DEFAULT_AUDIO_CLIP_ON_ERROR} 재생.");
                            onLoading?.Invoke(op2.Result);
                            _addressableHandle = op2;
                        }
                        else
                        {
                            onLoading?.Invoke(null);
                        }
                    };
                }
            };
        }

        public void Play()
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            if (_clip == null)
                _waitClip = true;
            else
                AudioSource.Play();
        }

        public void SetClip(AudioClip clip)
        {
            Clip = clip;
            _clipLoading = false;
        }

        public async void NotifyOnClipLoaded(UnityAction callback)
        {
            while (Clip == null)
                await Task.Yield();
            callback?.Invoke();
        }

        public IEnumerator WaitForClipLoading()
        {
            while (_clip == null && _clipLoading)
                yield return null;
        }

        public void Stop()
        {
            if (AudioSource != null)
                AudioSource.Stop();
            IsPlaying = false;
            IsPaused = false;
            OnStopPlaying();
        }

        public void OnStopPlaying()
        {
            OnStop?.Invoke();
            OnStop = null;
        }

        public void OnEndPlaying()
        {
            OnEnd?.Invoke();
            OnEnd = null;
        }

        public void Pause()
        {
            if (AudioSource != null)
                AudioSource.Pause();
            IsPaused = true;
            IsPlaying = false;
        }

        public void Resume()
        {
            if (AudioSource != null)
                AudioSource.UnPause();
            IsPaused = false;
            IsPlaying = true;
        }

        public void Update()
        {
            if (AudioSource == null)
            {
                DestroyThis();
                return;
            }

            if (_waitClip)
            {
                CheckClip();
                return;
            }

            if (!AudioSource.isPlaying)
                IsPlaying = false;

            if (_autoReleased && !IsPlaying && !IsPaused)
            {
                AudioSource.Stop();
                OnEndPlaying();
                DestroyThis();
            }
        }

        private void DestroyThis()
        {
            if (_addressableHandle.IsValid() && Clip != null)
                DelayReleaseStream.Subscribe(_ =>
                {
                    if (_addressableHandle.IsValid())
                        Addressables.Release(_addressableHandle);
                });
            if (AudioSource != null) Object.Destroy(AudioSource.gameObject);
            _soundService.Remove(this);
        }

        private void CheckClip()
        {
            if (_clip == null)
                return;
            _waitClip = false;
            AudioSource.Play();
        }

        public void Release()
        {
            AudioSource.Stop();
            OnEndPlaying();
            DestroyThis();
        }

        public IEnumerator WaitForEnd()
        {
            var waiting = false;
            OnEnd += () => waiting = true;
            yield return new WaitUntil(() => waiting);
        }
    }
}