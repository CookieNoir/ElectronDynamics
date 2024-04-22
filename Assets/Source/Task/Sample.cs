namespace ElectronDynamics.Task
{
    public readonly struct Sample
    {
        public readonly EdVector3 Position;
        public readonly EdVector3 Velocity;

        public Sample(EdVector3 position, EdVector3 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
