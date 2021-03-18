using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentalAssistant.Utils
{
    public class NoteValue
    {
        public EqualTemperamentNote note;
        public float duration;
        public float startTime;

        public NoteValue(EqualTemperamentNote note, float duration, float startTime)
        {
            this.note = note;
            this.duration = duration;
            this.startTime = startTime;
        }
    }
}
