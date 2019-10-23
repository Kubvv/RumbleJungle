﻿using CommonServiceLocator;
using System;
using System.Linq;
using System.Windows;

namespace RumbleJungle.Model
{
    public class JungleObject
    {
        private readonly GameModel gameModel = ServiceLocator.Current.GetInstance<GameModel>();
        private readonly JungleModel jungleModel = ServiceLocator.Current.GetInstance<JungleModel>();
        private readonly WeaponModel weaponModel = ServiceLocator.Current.GetInstance<WeaponModel>();

        public JungleObjectTypes JungleObjectType { get; private set; }
        public string Name { get; private set; }
        public Point Coordinates { get; private set; }
        public Statuses Status { get; private set; }

        public event EventHandler TypeChanged;
        public event EventHandler StatusChanged;

        public JungleObject(JungleObjectTypes jungleObjectType)
        {
            JungleObjectType = jungleObjectType;
            Name = Enum.GetName(typeof(JungleObjectTypes), jungleObjectType);
            Status = Configuration.VisibleItems.Contains(jungleObjectType) || Configuration.DebugMode ? Statuses.Visible : Statuses.Hidden;
        }

        public virtual void SetCoordinates(Point point)
        {
            Coordinates = point;
        }

        internal void SetStatus(Statuses status)
        {
            Status = status;
            StatusChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Changes the type of the jungle object to given type
        /// </summary>
        /// <param name="jungleObjectType">New type of jungle object</param>
        private void ChangeTypeTo(JungleObjectTypes jungleObjectType)
        {
            JungleObjectType = jungleObjectType;
            Name = Enum.GetName(typeof(JungleObjectTypes), jungleObjectType);
            TypeChanged?.Invoke(this, null);
        }

        public Point Action()
        {
            Point result = Coordinates;

            Random random = new Random();

            if (JungleObjectType == JungleObjectTypes.Elixir)
            {
                // Increase health by 25%-35%
                int healthAdded = random.Next(11) + 25;
                gameModel.Rambler.ChangeHealth(healthAdded);
            }
            else if (JungleObjectType == JungleObjectTypes.Compass)
            {
                // Point all monsters in a 5x5 square around the compass
                Point point = new Point(Coordinates.X - 2, Coordinates.Y - 2);
                for (int column = 0; column < 5; column++)
                {
                    for (int row = 0; row < 5; row++)
                    {
                        point.X = Coordinates.X + column - 2;
                        point.Y = Coordinates.Y + row - 2;
                        jungleModel.SetPointedAt(point, true);
                    }
                }
            }
            else if (JungleObjectType == JungleObjectTypes.Map)
            {
                // point nearest treasure
                JungleObject treasure = jungleModel.FindNearestTo(Coordinates, JungleObjectTypes.Treasure, Statuses.NotVisited);
                if (treasure != null)
                {
                    treasure.SetStatus(Statuses.Pointed);
                }
            }
            else if (JungleObjectType == JungleObjectTypes.Talisman)
            {
                // mark nearest hydra
                JungleObject hydra = jungleModel.FindNearestTo(Coordinates, JungleObjectTypes.Hydra, Statuses.NotVisited);
                if (hydra != null)
                {
                    hydra.SetStatus(Statuses.Marked);
                }
            }
            else if (JungleObjectType == JungleObjectTypes.LostWeapon)
            {
                // add random weapon
                weaponModel.ChangeRandomWeaponCount(1);
            }
            else if (JungleObjectType == JungleObjectTypes.Camp)
            {
                // add all weapons and 25-35% health
                // choose one option:
                //  30% more strength in next battle
                //  check four fields adjacent to camp
                //  additional 15% health
                //  double attack with random weapon
                weaponModel.ChangeAllWeaponsCount(1);
                int healthAdded = random.Next(6) + 40;
                gameModel.Rambler.ChangeHealth(healthAdded);

            }
            else if (JungleObjectType == JungleObjectTypes.Tent)
            {
                // add all weapons and 25-35% health
                weaponModel.ChangeAllWeaponsCount(1);
                int healthAdded = random.Next(6) + 35;
                gameModel.Rambler.ChangeHealth(healthAdded);

            }
            else if (JungleObjectType == JungleObjectTypes.ForgottenCity)
            {
                // TODO: fight with two random beasts, one after another
                // random weapon and two treasures pointed as a reward

                // add random weapon
                weaponModel.ChangeRandomWeaponCount(1);

                // point first treasure
                JungleObject treasure = jungleModel.FindNearestTo(Coordinates, JungleObjectTypes.Treasure, Statuses.Hidden);
                if (treasure != null)
                {
                    treasure.SetStatus(Statuses.Pointed);
                    // point second treasure
                    treasure = jungleModel.FindNearestTo(Coordinates, JungleObjectTypes.Treasure, Statuses.Hidden);
                    if (treasure != null)
                    {
                        treasure.SetStatus(Statuses.Pointed);
                    }
                }
            }
            else if (JungleObjectType == JungleObjectTypes.Natives)
            {
                // if rambler has some treasure
                // return one treasure to random empty field in the jungle
                if (jungleModel.CountOf(JungleObjectTypes.Treasure) < Configuration.JungleObjectsCount[JungleObjectTypes.Treasure])
                {
                    JungleObject emptyField = jungleModel.GetRandomJungleObject(JungleObjectTypes.EmptyField);
                    if (emptyField != null)
                    {
                        emptyField.ChangeTypeTo(JungleObjectTypes.Treasure);
                    }
                }
            }
            else if (JungleObjectType == JungleObjectTypes.Trap)
            {
                // decrease health by 25%-35%
                int healthSubtracted = random.Next(11) + 25;
                gameModel.Rambler.ChangeHealth(-healthSubtracted);
            }
            else if (JungleObjectType == JungleObjectTypes.Quicksand)
            {
                // jump to random hidden empty field
                JungleObject emptyField = jungleModel.GetRandomJungleObject(JungleObjectTypes.EmptyField);
                if (emptyField != null)
                {
                    result = emptyField.Coordinates;
                }
                // decrease health by 10
                gameModel.Rambler.ChangeHealth(-10);
            }
            else if (JungleObjectType == JungleObjectTypes.Treasure)
            {
                // do nothing
            }
            return result;
        }

        internal void CheckAdjacent()
        {
            jungleModel.SetPointedAt(new Point(Coordinates.X - 1, Coordinates.Y), false);
            jungleModel.SetPointedAt(new Point(Coordinates.X + 1, Coordinates.Y), false);
            jungleModel.SetPointedAt(new Point(Coordinates.X, Coordinates.Y - 1), false);
            jungleModel.SetPointedAt(new Point(Coordinates.X, Coordinates.Y + 1), false);
        }
    }
}
