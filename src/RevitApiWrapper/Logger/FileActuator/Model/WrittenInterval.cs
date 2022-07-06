using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.FileActuator.Model
{
    /// <summary>
    /// Logger Interval
    /// 写入周期
    /// </summary>
    public enum WrittenInterval
    {
        /// <summary>
        /// 不分周期
        /// </summary>
        None,
        
        /// <summary>
        /// 按天记录
        /// </summary>
        Day,
        
        /// <summary>
        /// 按周记录
        /// </summary>
        Week,
        
        /// <summary>
        /// 按月记录
        /// </summary>
        Month
    }
}
