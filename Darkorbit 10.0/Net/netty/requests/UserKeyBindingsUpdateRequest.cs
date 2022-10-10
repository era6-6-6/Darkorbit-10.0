namespace Darkorbit.Net.netty.requests
{
    class UserKeyBindingsUpdateRequest
    {
        public const short ID = 13016;

        public bool remove = false;
        public List<UserKeyBindingsModule> changedKeyBindings = new List<UserKeyBindingsModule>();

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            this.remove = parser.readBoolean();
            int i = 0;
            int length = parser.readInt();
            while (i < length)
            {
                parser.readShort();
                var ukbm = new UserKeyBindingsModule();
                ukbm.read(parser);
                this.changedKeyBindings.Add(ukbm);
                i++;
            }
        }
    }
}
