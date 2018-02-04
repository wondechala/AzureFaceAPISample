//|---------------------------------------------------------------|
//|                   AZURE FACE API WPF CLIENT                   |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                   AZURE FACE API WPF CLIENT                   |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace AzureFaceAPISample.Entities
{
    public class AgeGroupChart
    {
        #region Public Properties 

        public string NoOfAdolescence { get; set; }

        public string NoOfYoungAdult { get; set; }

        public string NoOfMiddleAgedAdult { get; set; }

        public string NoOfOldAdult { get; set; }

        public string Type
        {
            get { return Chart.PIE.EnumDescription(); }
        }

        #endregion
    }
}
