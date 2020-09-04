using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdk
{
    /// <summary>
    /// FullName： <see cref="Sdk.BaseEntity"/>
    /// 结构化的基础实体类
    /// Author:  XP
    /// Created: 2011/11/15
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : ECF.Entity
    {
        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <returns></returns>
        public virtual object GetObjectData()
        {
            return null;
        }
    }

}
