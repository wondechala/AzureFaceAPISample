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
    public class GenderChart
    {
        #region Public Properties 

        public string NoOfMale { get; set; }

        public string NoOfFemale { get; set; }

        public string Type
        {
            get { return Chart.BAR.EnumDescription(); }
        }

        #endregion
    }
}
