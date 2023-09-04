using UnityEngine.UIElements;

namespace UnityNuGetManager.UI.Manager
{
    public class LoadingSpinner : VisualElement
    {
        private float RotationSpeed { get; set; } = 360;
        private const int MsPerUpdate = 16;

        private IVisualElementScheduledItem _ScheduledUpdate;
        private bool _IsRunning = true;

        private void UpdateRotation()
        {
            Angle currentAngle = style.rotate.value.angle;
            float deltaRotation = RotationSpeed * (MsPerUpdate / 1000f); 
            var newAngle = new Angle(currentAngle.ToDegrees() + deltaRotation, AngleUnit.Degree);
            style.rotate = new Rotate(newAngle);
        }
        
        public LoadingSpinner()
        {
            _ScheduledUpdate = schedule.Execute(UpdateRotation).Every(MsPerUpdate);
        }

        public void Disable()
        {
            if (!_IsRunning) return;
            style.display = DisplayStyle.None;
            _ScheduledUpdate.Pause();
            _IsRunning = false;
        }

        public void Enable()
        {
            if (_IsRunning) return;
            style.display = DisplayStyle.Flex;
            _ScheduledUpdate.Resume();
            _IsRunning = true;
        }
        
        public new class UxmlFactory : UxmlFactory<LoadingSpinner, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlFloatAttributeDescription RotationSpeed { get; }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not LoadingSpinner spinner) return;

                spinner.RotationSpeed = RotationSpeed.GetValueFromBag(bag, cc);
            }

            public UxmlTraits()
            {
                RotationSpeed = new UxmlFloatAttributeDescription()
                {
                    name = "Rotation-Speed",
                    defaultValue = 360
                };
            }
        }
    }
}
