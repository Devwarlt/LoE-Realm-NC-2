#region

using System;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public class ActivateBoost
    {
        private readonly List<int> _amounts;

        public ActivateBoost()
        {
            _amounts = new List<int>();
        }

        public int GetBoost()
        {
            _amounts.Sort();

            var boost = 0;
            for (int i = 0; i < _amounts.Count; i++)
                boost += (int)(_amounts[_amounts.Count - 1 - i] * Math.Pow(.5, i));

            return boost;
        }

        public void Push(int amount)
        {
            _amounts.Add(amount);
        }

        public void Pop(int amount)
        {
            if (_amounts.Count > 0)
                _amounts.Remove(amount);
        }
    }
}