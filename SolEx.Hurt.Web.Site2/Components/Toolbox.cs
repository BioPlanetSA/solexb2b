using System.Collections.Generic;

namespace SolEx.Hurt.Web.Components
{
    public class Toolbox
    {
        public Toolbox()
        {
            Items = new List<ToolboxItem>();
            NoticeType = "message";
        }

        public string Title { get; set; }
        public string TitleSmall { get; set; }
        public string TitleIcon { get; set; }
        public string Action { get; set; }
        public string Notice { get; set; }
        public string NoticeType { get; set; }
        public string AspPrefix { get; set; }
        public HashSet<string> Params { get; set; }
        public List<ToolboxItem> Items { get; set; }
        public HashSet<string> Values { get; set; }

        public void AddButton(string action)
        {
            AddButton(action, "");
        }

        public void AddButton(string action, string command, string url=null)
        {
            ToolboxItem item = new ToolboxItem();
            item.Action = action;

            string cmd2 = "SubmitForm('{0}', {1});";
            string cmd = "if($('#aspnetForm')[0].checkValidity()){{show_exit_warning = false; SubmitForm('{0}', {1}); }}  return false;";
            switch (action)
            {
                case "new":
                    item.IconClass = "icon-32-new";
                    item.Id = "toolbar-new";
                    item.Name = "Dodaj";
                    item.Command = string.Format(cmd2, action, "false");
                    break;
                case "new_article":
                    item.IconClass = "icon-32-new";
                    item.Id = "toolbar-new";
                    item.Name = "Dodaj artykuł";
                    item.Command = string.Format(cmd2, action, "false");
                    break;
                case "new_article_category":
                    item.IconClass = "icon-32-new";
                    item.Id = "toolbar-new";
                    item.Name = "Dodaj kategorię";
                    item.Command = string.Format(cmd2, action, "false");
                    break;
                case "edit":
                    item.IconClass = "icon-32-edit";
                    item.Id = "toolbar-edit";
                    item.Name = "Edytuj";
                    item.Command = string.Format(cmd2, action, "true");
                    break;
                case "delete":
                    item.IconClass = "icon-32-delete";
                    item.Id = "toolbar-delete";
                    item.Name = "Usuń";
                    item.Command = string.Format(cmd2, action, "true");
                    break;
                case "delete_all":
                    item.IconClass = "icon-32-delete";
                    item.Id = "toolbar-delete";
                    item.Name = "Usuń wszystko";
                    item.Command = string.Format(cmd2, action, "false");
                    break;
                case "save":
                    item.IconClass = "icon-32-save";
                    item.Id = "toolbar-save";
                    item.Name = "Zapisz";
                    item.Command = string.Format(cmd, action, "false");
                    break;
                case "apply":
                    item.IconClass = "icon-32-apply";
                    item.Id = "toolbar-apply";
                    item.Name = "Zastosuj";
                    item.Command = string.Format(cmd, action, "false");
                    break;
                case "cancel":
                    item.IconClass = "icon-32-cancel";
                    item.Id = "toolbar-cancel";
                    item.Name = "Powrót";
                    item.Command = string.Format(cmd2, action, "false");
                    break;
                default:
                    item.IconClass = "icon-32-apply";
                    item.Id = "toolbar-apply";
                    item.Name = action;
                    item.Command = string.Format(cmd2, action, "true");
                    break;
            }

            if (!string.IsNullOrEmpty(url))
            {
                item.Url = url;
            }
            else
            {
                if (!string.IsNullOrEmpty(command))
                    item.Command = command;
            }

            Items.Add(item);
        }

        public void AddButton(string name, string action, string command, string clas, bool disabled)
        {
            ToolboxItem item = new ToolboxItem();
            item.Action = action;
            item.IconClass = clas;
            item.Name = name;
            item.Command = command;

            if (disabled)
            {
                //item.Command = "alert('Ta opcja jest niedostępna. Prawdopodobny brak uprawnień.');";
                item.IconClass += "_disabled";
                item.Id += "disabled";
            }
            Items.Add(item);
        }
    }
}
