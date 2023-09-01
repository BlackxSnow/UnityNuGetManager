using UnityNuGetManager.NuGetApi;

namespace UnityNuGetManager.Package.DependencyResolution
{
    public struct PreparedNode
    {
        public PendingNode Pending;
        public RegistrationsReponse Registrations;

        public PreparedNode(PendingNode pending, RegistrationsReponse registrations)
        {
            Pending = pending;
            Registrations = registrations;
        }
    }
}