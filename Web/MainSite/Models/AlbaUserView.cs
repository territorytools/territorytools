using System;

namespace WebUI.Models
{
    public class AlbaUserView
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public bool IsChecked { get; set; }
    }
}
