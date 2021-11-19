public class SettingsData
{
    //Declare variables used in the terrian generator
    private static SettingsData _instance = null;

    public int renderDistance;
    public float musicVolume;
    public float soundVolume;
    public float brightness;

    private SettingsData()
    {
        renderDistance = 4;
        musicVolume = 100.0f;
        soundVolume = 100.0f;
        //brightness = 
    }
    public static SettingsData getInstance()
    {
        //This code allows for there to only ever be one instance of this script as to not have duplicate settings files
        //If a script tries to create another one it will just be given this script
        //Files of this nature are called Singletons
        if (_instance == null) _instance = new SettingsData();
        return _instance;
    }
}