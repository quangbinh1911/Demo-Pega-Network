using System;

namespace Gameplay.Stats
{
    [Serializable]
    public class Modifier
    {
        public float value;
        public ModType type;
        public int order;

        public string guid;


        public Modifier()
        {
            guid = Guid.NewGuid().ToString();
        }

        public Modifier(float value, ModType type) : this()
        {
            this.value = value;
            this.type = type;
        }

        public Modifier(float value, ModType type, int order) : this(value, type)
        {
            this.order = order;
        }
    }
}