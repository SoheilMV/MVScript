namespace MVScript
{
    public class ScriptHandler
    {
        private ScriptOption _option;

        public ScriptHandler()
        {
            _option = new ScriptOption("Soheil", "MV");
        }

        public ScriptHandler(ScriptOption option)
        {
            _option = option ?? new ScriptOption("Soheil", "MV");
        }

        public Script Read(byte[] script)
        {
            string dec = _option.Decrypt(script);
            return new Script(dec);
        }

        public Script Read(string path)
        {
            byte[] script = File.ReadAllBytes(path);
            return Read(script);
        }

        public byte[] Write(Script script)
        {
            byte[] enc = _option.Encrypt(script.ToString());
            return enc;
        }

        public void Write(string path, Script script)
        {
            byte[] enc = Write(script);
            File.WriteAllBytes(path, enc);
        }
    }
}
