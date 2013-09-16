using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class TrackingViewModel
    {
        public List<ActionTrack> ActionTracks;
        public bool IsLimited;
        public int NumActionsShown;
    }
}