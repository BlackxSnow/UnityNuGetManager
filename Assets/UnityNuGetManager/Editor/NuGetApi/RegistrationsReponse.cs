namespace UnityNuGetManager.NuGetApi
{
    public class RegistrationsReponse
    {
        public long Count { get; set; }
        public RegistrationPage[] Items { get; set; }
    }
}