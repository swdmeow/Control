namespace Control.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;

    internal sealed class ServerHandler
    {
        public void OnRoundStarted()
        {

            foreach (var door in Door.List)
            {
                if(door.Type == DoorType.PrisonDoor)
                {
                    door.ChangeLock(DoorLockType.DecontLockdown);
                }
            }

            Timing.CallDelayed(control.Instance.Config.WaitingTimeToCassie, () =>
            {
                Cassie.Message(control.Instance.Config.StartCassie, false, false, true);
            });
            Timing.CallDelayed(control.Instance.Config.WaitingTimeToOpenPrisonDoors, () =>
            {
                foreach (var door in Door.List)
                {
                    if (door.Type == DoorType.PrisonDoor)
                    {
                        door.IsOpen = true;
                    }
                }
            });


            Timing.CallDelayed(control.Instance.Config.WaitingTimeToCassieContaimentBreach, () =>
            {
                Cassie.Message(control.Instance.Config.StartCassieContaimentBreach, false, false, true);
            });
        }
    }
}