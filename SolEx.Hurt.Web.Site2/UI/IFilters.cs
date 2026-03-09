namespace SolEx.Hurt.Web.Site2.UI
{
    public interface IFilters
    {
     
        System.Collections.Generic.List<string> Categories { get; set; }
        int CategoryId { get; set; }
        string Query { get; set; }
        string Script { get; set; }
        System.Collections.Generic.List<int> AttributesIds { get; set; }
    }
}
