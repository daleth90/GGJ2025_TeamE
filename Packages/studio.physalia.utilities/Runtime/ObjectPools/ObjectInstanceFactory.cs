namespace Physalia
{
    public abstract class ObjectInstanceFactory<T>
    {
        public abstract string Name { get; }

        public abstract T Create();
        public abstract void Reset(T instance);
        public abstract void Destroy(T instance);

        public virtual int GetExpandCount(int currentSize)
        {
            return 1;
        }
    }
}
