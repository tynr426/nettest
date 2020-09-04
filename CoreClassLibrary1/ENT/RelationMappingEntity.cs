using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ENT
{
    public class RelationMappingEntity
    {
        public ConnectorRelation ConnectorEntity { get; set; }
        public OrderRelationEntity OrderRelationEntity { get; set; }
        public Dictionary<int, int> ConnectorMapping = new Dictionary<int, int>();
        public Dictionary<int, int> ProductMapping = new Dictionary<int, int>();
        public Dictionary<int, int> GoodsMapping = new Dictionary<int, int>();
        public Dictionary<int, int> OrderMapping = new Dictionary<int, int>();
        public Dictionary<string, string> OrderOddNumberMapping = new Dictionary<string, string>();
        public Dictionary<int, int> DispatchMapping = new Dictionary<int, int>();
    }
}
