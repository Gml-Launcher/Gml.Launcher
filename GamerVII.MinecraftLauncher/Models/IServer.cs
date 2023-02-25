using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GamerVII.MinecraftLauncher.Models
{
    public interface IServer
    {
        public string Name { get; set; }
        public ImageSource Image { get; set; }
    }
}
