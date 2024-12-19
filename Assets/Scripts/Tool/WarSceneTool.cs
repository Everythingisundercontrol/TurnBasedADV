using System.Collections.Generic;

namespace Tool
{
    public static class WarSceneTool
    {
        public static void SelectPointByUnitAttr<T>(IEnumerable<string> pointSet, IReadOnlyDictionary<string, Point> points,
            IReadOnlyDictionary<string, Unit> units, T attr)
        {
            
        }

        /// <summary>
        /// 筛选有unit的point
        /// </summary>
        public static HashSet<string> SelectPointHoldUnit(IEnumerable<string> pointSet, IReadOnlyDictionary<string, Point> points)
        {
            var res = new HashSet<string>();
            foreach (var pointID in pointSet)
            {
                if (points.ContainsKey(pointID) && !string.IsNullOrEmpty(points[pointID].unitID))
                {
                    res.Add(pointID);
                }
            }

            return res;
        }

        /// <summary>
        /// 获取指定pointId在Layer内的所有pointId
        /// </summary>
        public static HashSet<string> GetPointsWithinNLayers(Dictionary<string, PointData> pointData, string startId, int layers)
        {
            var curLayer = 0;
            var visited = new HashSet<string>(); //访问过的点
            var result = new HashSet<string>(); //指定步数内可达的所有pointID
            var curList = new List<string>(); // 队列中存储元组，包含点和当前层级
            var nextList = new List<string>(); // 下一层
            curList.Add(startId);

            while (curLayer < layers)
            {
                FixNextList(pointData, curList, nextList, visited);
                foreach (var pointId in nextList)
                {
                    result.Add(pointId);
                }

                curList = nextList;
                nextList.Clear();
                curLayer++;
            }

            return result;
        }

        /// <summary>
        /// 修改nextList
        /// </summary>
        private static void FixNextList(IReadOnlyDictionary<string, PointData> pointData, IEnumerable<string> curList, ICollection<string> nextList,
            ISet<string> visited)
        {
            foreach (var pointId in curList)
            {
                var addList = DoMove(pointData, pointId, visited);
                foreach (var addPointId in addList)
                {
                    nextList.Add(addPointId);
                }
            }
        }

        /// <summary>
        /// 指定点的附件所有未访问过的点
        /// </summary>
        private static IEnumerable<string> DoMove(IReadOnlyDictionary<string, PointData> pointData, string startId, ISet<string> visited)
        {
            var result = new List<string>();
            if (visited.Contains(startId))
            {
                return result;
            }

            visited.Add(startId);
            var currentPoint = pointData[startId];

            foreach (var nextId in currentPoint.nextPoints)
            {
                if (!pointData.ContainsKey(nextId) || visited.Contains(nextId))
                {
                    continue;
                }

                result.Add(nextId);
                visited.Add(nextId);
            }

            return result;
        }
    }
}