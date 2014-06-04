using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.Login
{
    public class RegisterAccount : ObservableObject
    {
        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                Set<string>(() => Username, ref _Username, value);
            }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                Set<string>(() => Password, ref _Password, value);
            }
        }
        private string _ConfirmPassword;
        public string ConfirmPassword
        {
            get { return _ConfirmPassword; }
            set
            {
                Set<string>(() => ConfirmPassword, ref _ConfirmPassword, value);
            }
        }
    }
}
