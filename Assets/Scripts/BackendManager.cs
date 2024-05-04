using Tools;
using Firebase.Auth;
using Firebase.Database;

public class BackendManager : SingletonMonoBehaviour<BackendManager>
{
    public FirebaseAuth Auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;
}



