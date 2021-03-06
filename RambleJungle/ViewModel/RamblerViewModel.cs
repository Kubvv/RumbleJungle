﻿using RambleJungle.Model;
using System;

namespace RambleJungle.ViewModel
{
    public class RamblerViewModel : JungleObjectViewModel
    {
        private readonly Rambler rambler;

        public double Strength => rambler.Strength;
        public bool Visible => rambler.Visible;
        public new double Width => Math.Max(base.Width - 1, 0);
        public new double Height => Math.Max(base.Height - 1, 0);

        public RamblerViewModel(GameModel gameModel) : base(gameModel?.Rambler)
        {
            rambler = gameModel?.Rambler;
            if (rambler != null)
            {
                rambler.Moved += RamblerMoved;
                rambler.HealthChanged += RamblerHealthChanged;
                rambler.StrengthChanged += RamblerStrengthChanged;
                rambler.VisibleChanged += RamblerVisibleChanged;
            }
        }

        public event EventHandler Moved;

        private void RamblerMoved(object sender, EventArgs e)
        {
            Update();
            Moved?.Invoke(this, null);
        }
        private void RamblerHealthChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Health));
        }

        private void RamblerStrengthChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Strength));
        }

        private void RamblerVisibleChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Visible));
        }
    }
}
