using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class RepeatTimes : DecoratorNode {

        public bool restartOnSuccess = true;
        public bool restartOnFailure = false;
        public int repeatTimes = 5;
        
        protected override void OnStart() {

        }

        protected override void OnStop() {

        }

        protected override State OnUpdate() {
            switch (child.Update()) {
                case State.Running:
                    break;
                case State.Failure:
                    if (restartOnFailure && repeatTimes > 0) {
                        repeatTimes--;
                        return State.Running;
                    } else {
                        return State.Failure;
                    }
                case State.Success:
                    if (restartOnSuccess && repeatTimes > 0) {
                        repeatTimes--;
                        return State.Running;
                    } else {
                        return State.Success;
                    }
            }
            return State.Running;
        }
    }

    
}
