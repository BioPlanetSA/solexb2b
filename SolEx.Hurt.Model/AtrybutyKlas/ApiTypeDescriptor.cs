namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public enum ApiGrupy
    {
        Klienci,Dokumenty,Produkty,Koszyk,Logowanie
    }

    public class ApiPropertyDescriptor : System.Attribute
    {
        private string sName;
        public string Opis
        {
            get { return sName; }
            set { sName = value; }
        }
        public ApiPropertyDescriptor(string nazwa)
        {
            sName = nazwa;
        }
    }

    public class ApiTypeDescriptor : System.Attribute
    {
        private string sName;
        private string sDescription;
        private string url;
        private string _result;
        private ApiGrupy _group;

        public ApiGrupy Group
        {
            get { return _group; }
        }
        public string Result
        {
            get { return _result; }
        }
        public string Url
        {
          get { return url; }
          set { url = value; }
        }
        public string Name
        {
            get { return sName; }
            set { sName = value; }
        }
        public string Description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }
        public ApiTypeDescriptor(ApiGrupy _group, string _name, string _desc)
            : this(_group, _name)
        {
            this.Description = _desc;
        }
        public ApiTypeDescriptor(ApiGrupy _group, string _name) { this.Name = _name;  this._group = _group; }
        public ApiTypeDescriptor(ApiGrupy _group, string _name, string _desc, string _result)
            : this(_group, _name, _desc)
        {
            this._result = _result;
        }
    }
  
}
