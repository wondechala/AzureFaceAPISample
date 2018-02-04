//|---------------------------------------------------------------|
//|                            COMMON                             |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                            COMMON                             |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Common
{
    /// <summary>
    /// SignalR EventName enumeration.
    /// Note : The Enum Description should match the defined SignalR method
    /// </summary>
    public enum EventName
    {
        /// <summary>
        /// Unknown enum
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// On Message Listened(onMessageListened) enum value. For general purpose
        /// </summary>
        [Description("onMessageListened")]
        ON_MESSAGE_LISTENED = 1,

        /// <summary>
        /// On Chart Produced(onChartProduced) enum value.
        /// </summary>
        [Description("onChartProduced")]
        ON_CHART_PRODUCED = 2,

        /// <summary>
        /// On Photo Detected(onPhotoDetected) enum value.
        /// </summary>
        [Description("onPhotoDetected")]
        ON_PHOTO_DETECTED = 3,

        /// <summary>
        /// On Exception(onException) enum value.
        /// </summary>
        [Description("onException")]
        ON_EXCEPTION = 4
    }
}
