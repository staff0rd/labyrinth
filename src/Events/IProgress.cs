using System.Threading.Tasks;

namespace Events
{
    public interface IProgress {

        Task New();
        Task Set(int value);
        Task Set(int current, int total);
    }
}