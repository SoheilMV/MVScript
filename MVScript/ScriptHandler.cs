namespace MVScript
{
    public class ScriptHandler
    {
        public ISecurity? Security { get; set;}

        public ScriptHandler(ISecurity security)
        {
            Security = security;
        }

        public ScriptHandler()
        {
        }

        public Script Read(string path)
        {
            if (Security != null)
            {
                byte[] script = File.ReadAllBytes(path);
                string dec = Security.Decrypt(script);
                return new Script(dec);
            }
            else
            {
                string script = File.ReadAllText(path);
                return new Script(script);
            }
        }

        public void Write(string path, Script script)
        {
            if (Security != null)
            {
                byte[] enc = Security.Encrypt(script.ToString());
                File.WriteAllBytes(path, enc);
            }
            else
            {
                File.WriteAllText(path, script.ToString());
            }
        }
    }
}
