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
            this.DebugHandGeometryImageSource = info.DebugHandGeometryImage == null ? "" : Convert.ToBase64String(info.DebugHandGeometryImage);
            this.DebugPalmImageSource = info.DebugPalmImage == null ? "" : Convert.ToBase64String(info.DebugPalmImage);
            this.OriginalImageSource = info.OriginalImage == null ? "" : Convert.ToBase64String(info.OriginalImage);
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
