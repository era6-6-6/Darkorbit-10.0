namespace Darkorbit.Net.netty.requests
{
    class KillscreenRequest
    {
        public static short ID = 25971;
        public KillScreenOptionTypeModule selection;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            parser.readShort();
            selection = new KillScreenOptionTypeModule(parser.readShort());
        }
    }
}
