namespace Events
{
    public interface IProgress {

        void New();
        void Set(int value);
        void Set(int current, int total);
    }
}