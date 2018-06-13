using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage.Model
{
    public interface IContentModel
    {
        int Id { get; set; }

        DateTime CDate { get; set; }

        string Url { get; set; }
    }
}