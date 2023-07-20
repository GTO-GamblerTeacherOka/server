using MasterMemory;
namespace VIRCE_server.DataBase;

public class UserDataBase
{
    private static UserDataBase _instance;

    public static UserDataBase Instance
    {
        get
        {
            return _instance ??= new UserDataBase();
        }
    }
    
    private UserDataBase()
    {
        
    }
}