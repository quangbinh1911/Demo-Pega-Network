using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Gameplay;
using UnityEngine;

namespace UI
{
    [DefaultExecutionOrder(Int32.MinValue)]
    public class RacingTrackPanel : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [Space, SerializeField] private EnhancedScroller scroller;
        [SerializeField] private RacingTrackCell cellPrefab;
        [SerializeField] private float cellSize = 100f;

        // key = connection id
        private readonly Dictionary<int, RacingTrackCell> _cells = new();


        public async void OnEnable()
        {
            await UniTask.WaitUntil(() => GameplayController.Singleton != null);
            await UniTask.WaitUntil(() => GameplayController.Singleton.Players.IsNetworkInitialized);
            
            GameplayController.Singleton.PlayersCountChanged += ReloadCells;
            GameplayController.Singleton.VelocityChanged += OnVelocityChanged;

            ReloadCells();
        }

        public void OnDisable()
        {
            GameplayController.Singleton.PlayersCountChanged -= ReloadCells;
            GameplayController.Singleton.VelocityChanged -= OnVelocityChanged;
        }


        private void OnVelocityChanged(VelocityChanged changed)
        {
            if (!_cells.ContainsKey(changed.ConnectionID)) return;
            _cells[changed.ConnectionID].SetVelocity(changed.Velocity);
        }


        private List<KeyValuePair<int, PlayerInfo>> _players = new();

        private void ReloadCells()
        {
            _players = GameplayController.Singleton.Players.ToList();
            
            _cells.Clear();

            if (scroller.Delegate == null) scroller.Delegate = this;
            else scroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _players.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cell = (RacingTrackCell)scroller.GetCellView(cellPrefab);

            var info = _players[dataIndex];

            cell.SetInfo(info.Value);
            cell.SetVelocity(GameplayController.Singleton.GetVelocity(info.Key));

            _cells.Add(info.Key, cell);


            return cell;
        }
    }
}