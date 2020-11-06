using AccessPointControlClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.DTO
{
    public class DebugInfoDTO
    {
        public string Info { get; set; }
        public byte[] DebugHandGeometryImage { get; set; }
        public byte[] DebugPalmImage { get; set; }

        public byte[] OriginalImage { get; set; }

    }
}
