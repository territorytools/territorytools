using System;
using System.Collections.Generic;
using System.Text;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AddUserRequest
    {
        //
        //?mod=users&cmd=userAdd&
        //user_name=marcdurhamthree&user_real_name=Marc+Durham&
        //user_email=marc%40x-fire.us&user_telephone=2066595503&
        //pw1=EEE103NDhall&pw2=EEE103NDhall&user_role=20&welcome=true
        public string Password { get; set; }
        public UserRoles UserRole { get; set; }
        public bool SendWelcomeEmail { get; set; }
        public string UserTelephone { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
    }
}
