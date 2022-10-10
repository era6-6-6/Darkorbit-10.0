namespace Darkorbit.Game.Movements
{
    class Movement
    {
        public static void Move(Character character, Position destination, int a = 1000)
        {
            if (character.Speed <= 0)
            {
                character.Moving = false;
                return;
            }

            character.MovementTime = GetTime(character, destination, a);

            character.MovementStartTime = DateTime.Now;
            character.Moving = true;

            character.SendCommandToInRangePlayers(MoveCommand.write(character.Id, destination.X, destination.Y, character.MovementTime));
            foreach(Character c in character.Spacemap.Characters.Values)
            {
                if(c.SelectedCharacter == character)
                {
                    if(c is Player p)
                    {
                        p.SendCommand(MoveCommand.write(character.Id, destination.X, destination.Y, character.MovementTime));
                    }
                }
            }
        }

        public static int GetTime(Character character, Position destination, int a = 1000)
        {
            try
            {
                character.OldPosition = ActualPosition(character);

                var destinationPosition = destination;
                character.Destination = destinationPosition;

                character.Direction = new Position(destinationPosition.X - character.OldPosition.X, destinationPosition.Y - character.OldPosition.Y);

                var distance = destinationPosition.DistanceTo(character.OldPosition);
                var time = Math.Round(distance / character.Speed * a);

                return (int)time;
            }
            catch (Exception e)
            {
                Out.WriteLine("GetTime void exception: " + e, "Movement.cs");
                Logger.Log("error_log", $"- [Movement.cs] GetTime void exception: {e}");
            }
            return -1;
        }

        public static Position ActualPosition(Character character)
        {
            Position actualPosition;

            if (character.Moving || character.Storage.underDCR_250)
            {
                var timeElapsed = (DateTime.Now - character.MovementStartTime).TotalMilliseconds;

                if (timeElapsed < character.MovementTime)
                {
                    actualPosition = new Position((int)Math.Round(character.OldPosition.X + (character.Direction.X * (timeElapsed / character.MovementTime))),
                            (int)Math.Round(character.OldPosition.Y + (character.Direction.Y * (timeElapsed / character.MovementTime))));
                }
                else
                {
                    character.Moving = false;
                    actualPosition = character.Destination;
                }
            }
            else
            {
                actualPosition = character.Position;
            }

            character.Position = actualPosition;

            return actualPosition;
        }
    }
}
