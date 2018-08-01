using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{

    public interface IGamingSysComponent
    {
        void Enter();
        void EnterFinish();
        void Exit();
        void ExitFinish();
        void Update(FP deltaTime);
    }
}
