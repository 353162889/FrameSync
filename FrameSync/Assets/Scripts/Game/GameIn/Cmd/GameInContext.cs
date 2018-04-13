using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GameInContext : ICommandContext
    {
        public int sceneId { get; set; }
        public void Dispose()
        {
        }
    }
}
