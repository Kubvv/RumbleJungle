﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RumbleJungle.Model
{
    public class JungleObject
    {
        public Point Coordinates { get; private set; } 
        internal void SetCoordinates(Point point)
        {
            Coordinates = point;
        }
    }
}