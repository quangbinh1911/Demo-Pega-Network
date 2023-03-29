using Cinemachine;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Component.Prediction;
using UnityEngine;

namespace Gameplay.UX
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase introCam;
        [SerializeField] private new CinemachineVirtualCamera followCam;


        private async void Awake()
        {
            await UniTask.WaitUntil(() => GameplayController.Singleton.Pegas.IsNetworkInitialized && InstanceFinder.ClientManager.Connection.IsActive);
            
            GameplayController.Singleton.onStateChanged += OnStateChanged;
            OnStateChanged(GameplayController.Singleton.state, GameplayController.Singleton.state, false);
        }

        private void OnDestroy()
        {
            GameplayController.Singleton.onStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState oldState, GameState state, bool arg3)
        {
            switch (state)
            {
                default:
                case GameState.Waiting:
                {
                    introCam.gameObject.SetActive(true);
                    followCam.gameObject.SetActive(false);
                    break;
                }

                case GameState.Playing:
                {
                    var connection = InstanceFinder.ClientManager.Connection;
                    var pega = GameplayController.Singleton.Pegas[connection.ClientId];

                    if (pega)
                    {
                        followCam.Follow = pega.GetComponent<PredictedObject>().GetGraphicalObject();
                    }


                    introCam.gameObject.SetActive(false);
                    followCam.gameObject.SetActive(true);

                    break;
                }
            }
        }
    }
}