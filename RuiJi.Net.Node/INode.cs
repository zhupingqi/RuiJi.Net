using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public interface INode
    {
        string BaseUrl { get; }

        NodeTypeEnum NodeType { get; }

        DateTime StartTime { get; }

        bool IsLeader { get; }

        NodeData GetData(string path);

        void SetData(string path, string data, int version = -1);

        void Start();

        void Stop();
    }
}