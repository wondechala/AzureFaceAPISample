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
    public class IdentifyFace
    {
        #region Public Properties 

        public bool IsFound { get; set; } = false;

        public string Base64ImageString { get; set; }

        public string PersonName { get; set; }

        public bool IsCompleted { get; set; } = false;

        #endregion
    }
}
