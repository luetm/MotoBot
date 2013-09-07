
using MotoBotCore.Enums;
namespace MotoBotCore.Interfaces
{
    public interface IUser
    {
        string Name { get; }
        UserMode Mode { get; }
    }
}
