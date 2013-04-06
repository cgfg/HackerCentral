using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class PointsViewModel
    {
        public List<Point> points;
        public Point point; // Passed to _CreatePoint partial view
    }
}