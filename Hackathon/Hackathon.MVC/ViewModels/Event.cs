using System;

namespace Hackathon.MVC.ViewModels
{
    public class Event
    {
        public virtual string title { get; set; }

        public virtual DateTime? start { get; set; }

        public virtual DateTime? end { get; set; }
    }
}