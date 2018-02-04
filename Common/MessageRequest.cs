//|---------------------------------------------------------------|
//|                          COMMON                               |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                          COMMON                               |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Message Request class
    /// </summary>
    public class MessageRequest
    {
        #region Public Properties       
        
        /// <summary>
        /// Get or set Message value
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get or set EventName
        /// </summary>
        public EventName EventName { get; set;}

        /// <summary>
        /// Message Request class
        /// </summary>
        public MessageRequest()
        {

        }

        #endregion
    }
}
