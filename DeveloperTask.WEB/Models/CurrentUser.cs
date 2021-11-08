using DeveloperTask.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeveloperTask.WEB.Models
{
    public class CurrentUser : User
    {
        private static CurrentUser m_Instance;
        public static CurrentUser Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CurrentUser() { Id =0};

                return m_Instance;
            }
        }
    }
}