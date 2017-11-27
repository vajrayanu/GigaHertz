using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class TrackingCounter
    {
        [Key]
        public int kTrackingCounterId { get; set; }


        public int iCounter { get; set; }
    }
}
