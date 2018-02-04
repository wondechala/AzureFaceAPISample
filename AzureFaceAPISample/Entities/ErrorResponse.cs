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
    public class ErrorResponse
    {
        #region Private Members 

        private string _errrorMessage = string.Empty;

        #endregion

        #region Public Properties 

        public string ErrorMessage
        {
            get { return _errrorMessage ?? string.Empty; }
            set { _errrorMessage = value; }
        }

        #endregion

    }
}
