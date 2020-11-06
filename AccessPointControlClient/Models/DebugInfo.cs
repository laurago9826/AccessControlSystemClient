using AccessPointControlClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AccessPointControlClient.Models
{
    public class DebugInfo
    {
        public string Info { get; set; }
        public string DebugHandGeometryImageSource { get; set; }
        public string DebugPalmImageSource { get; set; }

        public string OriginalImageSource { get; set; }

        public DebugInfo()
        {
                
        }

        public DebugInfo(DebugInfoDTO info)
        {
            this.Info = info.Info;
            this.DebugHandGeometryImageSource = Convert.ToBase64String(info.DebugHandGeometryImage);
            this.DebugPalmImageSource = Convert.ToBase64String(info.DebugPalmImage);
            this.OriginalImageSource = Convert.ToBase64String(info.OriginalImage);
        }

        public static DebugInfo CreateEmptyDebugInfo()
        {
            return new DebugInfo()
            {
                DebugHandGeometryImageSource = "",
                DebugPalmImageSource = "",
                Info = "",
                OriginalImageSource = ""
            };
        }
    }
}
