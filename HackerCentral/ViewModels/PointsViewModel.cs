using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.ViewModels
{
    public class PointsViewModel
    {
        public List<Point> points;
        public Point point; // Passed to _CreatePoint partial view so validation works
        public List<NestItem> nestedPoints;
        public List<Point> visiblePoints;
        public List<Point> topPoints;

        public PointsViewModel(List<Point> points)
        {
            visiblePoints = new List<Point>();
            this.points = points.OrderBy(p => p.id).ToList();
            //this.topPoints = points.Where(p => p.).OrderByDescending(p => p.quality).Take(10).ToList();
            // Create a blank (but not null) point
            point = new Point()
            {
                id = 0,
                summary = "",
                full_text = "",
                category = 0
            };

            // Create nestedPoints list
            nestedPoints = new List<NestItem>();

            // Find all points with parent id of 0 and make a nest item.  For each, recursively add children that match
            foreach(var top in points.Where(p=>p.parent_id == 0) )
            {
                visiblePoints.Add(top);
                nestedPoints.Add(new NestItem()
                {
                    Parent = top,
                    Children = GetChildren(top, points)               
                });
            }

            nestedPoints = nestedPoints.OrderBy(n => n.Parent.id).ToList();
            topPoints = visiblePoints.OrderByDescending(p => p.quality).Take(10).ToList();
        }

        private List<NestItem> GetChildren(Point parent, List<Point> allPoints)
        {
            // End case: if allPoints does not have a point with the parent_id = parent.id
            if (allPoints.Where(p => p.parent_id == parent.id).Count() <= 0)
                return new List<NestItem>(); // Empty list

            // Recursive case: 
            var nestList = new List<NestItem>();

            foreach (var top in allPoints.Where(p => p.parent_id == parent.id))
            {
                visiblePoints.Add(top);
                nestList.Add(new NestItem()
                {
                    Parent = top,
                    Children = GetChildren(top, allPoints)
                });
            }
           
            return nestList;
        }

        public List<SelectListItem> GetParentIDList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "None", Value = "0" });

            foreach (var nest in nestedPoints)
            {
                list.AddRange(GetListItems(nest, 0));
            }

            return list;
        }


        private List<SelectListItem> GetListItems(NestItem nest, int indentLevel)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = new String('-', indentLevel * 2) + nest.Parent.summary, Value = nest.Parent.id.ToString() });
            if (nest.HasChildren())
            {
                foreach (var n in nest.Children)
                {
                    list.AddRange(GetListItems(n, ++indentLevel));
                }
            }
            return list;
        }



        // Class allows recursive nesting of points
        public class NestItem
        {
            public Point Parent;
            public List<NestItem> Children;

            public bool HasChildren() { return Children.Count > 0; }
        }

    }
}