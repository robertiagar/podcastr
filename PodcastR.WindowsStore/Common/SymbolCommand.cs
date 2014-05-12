using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace PodcastR.WindowsStore.Common
{
    public class SymbolCommand : ObservableObject
    {
        private SymbolIcon _Icon;
        private string _Label;

        public SymbolCommand(ICommand command, Symbol symbol, string label)
        {
            this.Command = command;
            this.Icon = new SymbolIcon(symbol);
            this.Symbol = symbol;
            this.Label = label;
        }

        public ICommand Command { get; private set; }

        public SymbolIcon Icon
        {
            get { return _Icon; }
            set
            {
                Set<SymbolIcon>(() => Icon, ref _Icon, value);
            }
        }

        public string Label
        {
            get { return _Label; }
            set
            {
                Set<string>(() => Label, ref _Label, value);
            }
        }

        private Symbol _Symbol;
        public Symbol Symbol
        {
            get { return _Symbol; }
            set
            {
                Set<Symbol>(() => Symbol, ref _Symbol, value);
                this.Icon.Symbol = value;
            }
        }
    }
}
