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

namespace AzureFaceAPISample.Entities
{
    public class TrainFace
    {
        #region Public Properties 

        public string ImageDirectoryPath { get; set; }

        public string ImageFilterType { get; private set; } = "*.jpg";

        public string PersonGroupID { get; set; }

        public string PersonGroupName { get; set; }

        public string PersonName { get; set; }

        #endregion

    }
}
