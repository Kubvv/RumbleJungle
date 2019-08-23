﻿using CommonServiceLocator;
using GalaSoft.MvvmLight;
using RumbleJungle.Model;
using System;

namespace RumbleJungle.ViewModel
{
    public class JungleObjectStatusViewModel : ViewModelBase
    {
        private readonly JungleModel jungleModel = ServiceLocator.Current.GetInstance<JungleModel>();

        private JungleObject jungleObject;

        public string Name => jungleObject.Name;
        public string Shape => $"/RumbleJungle;component/Images/{jungleObject.Name}.svg";
        public int Count => jungleModel.CountOf(jungleObject.JungleObjectType);

        public JungleObjectStatusViewModel(JungleObject firstJungleObject)
        {
            this.jungleObject = firstJungleObject;
            foreach (JungleObject jungleObject in jungleModel.GetJungleObjects(firstJungleObject.JungleObjectType))
            {
                jungleObject.StatusChanged += StatusChanged;
            }
        }

        private void StatusChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Count");
        }
    }
}