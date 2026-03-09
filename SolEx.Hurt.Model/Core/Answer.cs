namespace SolEx.Hurt.Model.Core
{
    /// <summary>
    /// Model odpowiedzi do komunikatów
    /// </summary>
    public class Answer
    {

        public Answer(string name, int id)
        {

            Id = id;
            Name = name;
        }
        public Answer()
        {
        }
        public string Html { get; set; }

        public int Id { get; set; }

        public string Desc { get; set; }

        public string Name { get; set; }
    }
}
