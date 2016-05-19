namespace SqlHelper.Class
{
    public class PrimaryKey
    {
        public string Tabela { get; private set; }

        public string Coluna { get; private set; }


        public bool Identity { get; private set; }

        public PrimaryKey(string tabela, string coluna, bool identity)
        {
            this.Tabela = tabela;
            this.Coluna = coluna;
            this.Identity = identity;
        }
    }
}