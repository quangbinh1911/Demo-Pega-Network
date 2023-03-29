using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using TMPro;
using UnityEngine;

namespace Gameplay.Input
{
    public class NotifyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text notifiesTxt;

        //key is Guid of notify, value is content of notify
        private static List<AddNotify> _notes = new();


        private void OnEnable()
        {
            InstanceFinder.ClientManager.RegisterBroadcast<AddNotify>(AddNotify);
            InstanceFinder.ClientManager.RegisterBroadcast<RemoveNotify>(RemoveNotify);
        }

        private void OnDisable()
        {
            if (InstanceFinder.ClientManager)
            {
                InstanceFinder.ClientManager.UnregisterBroadcast<AddNotify>(AddNotify);
                InstanceFinder.ClientManager.UnregisterBroadcast<RemoveNotify>(RemoveNotify);
            }
        }


        private void AddNotify(AddNotify notify)
        {
            _notes.Add(notify);
            OnNotifyChange();
        }

        private void RemoveNotify(RemoveNotify notify)
        {
            _notes = _notes.Where(note => note.Guid != notify.Guid).ToList();
            OnNotifyChange();
        }

        private void OnNotifyChange()
        {
            notifiesTxt.SetText(string.Empty);

            foreach (var note in _notes)
            {
                notifiesTxt.text += $"{note.Content}" + Environment.NewLine;
            }
        }
    }
}