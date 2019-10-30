﻿using CommonServiceLocator;
using System;
using System.Windows;

namespace RumbleJungle.Model
{
    public class Rambler : LivingJungleObject
    {
        public double Strength { get; private set; } = 1;

        public bool Visible { get; private set; }

        public Rambler() : base(JungleObjectTypes.Rambler)
        {
        }

        public new void Reset()
        {
            ChangeHealth(Configuration.DebugMode ? 50 : 100);
            SetVisible(true);
        }

        public event EventHandler Moved;

        public override void SetCoordinates(Point point)
        {
            base.SetCoordinates(point);
            Moved?.Invoke(this, null);
            JungleModel jungleModel = ServiceLocator.Current.GetInstance<JungleModel>();
            JungleObject jungleObject = jungleModel.GetJungleObjectAt(point);
            jungleObject.SetStatus(Statuses.Visited);
        }

        public event EventHandler StrengthChanged;

        public void SetStrength(double newStrength)
        {
            Strength = newStrength;
            StrengthChanged?.Invoke(this, null);
        }

        public event EventHandler VisibleChanged;

        public void SetVisible(bool visible)
        {
            Visible = visible;
            VisibleChanged?.Invoke(this, null);
        }
    }
}
