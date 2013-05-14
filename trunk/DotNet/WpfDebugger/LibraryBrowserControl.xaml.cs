﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Disassembler;
using Disassembler.Omf;

namespace WpfDebugger
{
    /// <summary>
    /// Interaction logic for LibraryBrowserControl.xaml
    /// </summary>
    public partial class LibraryBrowserControl : UserControl
    {
        public LibraryBrowserControl()
        {
            InitializeComponent();
#if false
        typeof(VirtualizingStackPanel).GetProperty("IsPixelBased", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, true, null);
#endif
        }

        private ObjectLibrary library;

        public ObjectLibrary Library
        {
            get { return library; }
            set
            {
                library = value;
                if (library == null)
                {
                    this.DataContext = null;
                }
                else
                {
                    var viewModel = new LibraryBrowserViewModel(library);
                    this.DataContext = viewModel;
                    myTreeView.ItemsSource = viewModel.Libraries;
                }
            }
        }

        private void TreeView_ItemActivate(object sender, EventArgs e)
        {
            if (sender is LibraryBrowserViewModel.LibraryItem)
            {
            }
            else if (sender is LibraryBrowserViewModel.ModuleItem)
            {
            }
            else if (sender is LibraryBrowserViewModel.SymbolItem)
            {
                MessageBox.Show("Show info about " + sender.ToString());
            }
        }
    }

    internal class LibraryBrowserViewModel
    {
        public LibraryItem[] Libraries { get; private set; }
        public LibraryItem Library { get { return Libraries[0]; } }

        public LibraryBrowserViewModel(ObjectLibrary library)
        {
            this.Libraries = new LibraryItem[1] { new LibraryItem(library) };    
        }

        internal class LibraryItem : ITreeNode
        {
            public ObjectLibrary Library { get; private set; }
            public ObservableCollection<ModuleItem> Modules { get; private set; }
            public string Name { get { return "Library"; } }
            
            public LibraryItem(ObjectLibrary library)
            {
                if (library == null)
                    throw new ArgumentNullException("library");

                this.Library = library;
                this.Modules = 
                    new ObservableCollection<ModuleItem>(
                        from module in library.Modules
                        select new ModuleItem(module));
            }

            public string Text
            {
                get { return "Library"; }
            }

            public bool HasChildren
            {
                get { return Modules.Count > 0; }
            }

            public IEnumerable<ITreeNode> GetChildren()
            {
                return Modules;
            }
        }

        internal class ModuleItem : ITreeNode
        {
            public ObjectModule Module { get; private set; }
            public string Name { get { return Module.ObjectName; } }
            public ObservableCollection<SymbolItem> Symbols { get; private set; }

            public ModuleItem(ObjectModule module)
            {
                if (module == null)
                    throw new ArgumentNullException("module");

                this.Module = module;
                this.Symbols = 
                    new ObservableCollection<SymbolItem>(
                        from publicName in module.PublicNames
                        select new SymbolItem(publicName));
            }

            public string Text
            {
                get { return this.Name; }
            }

            public bool HasChildren
            {
                get { return Symbols.Count > 0; }
            }

            public IEnumerable<ITreeNode> GetChildren()
            {
                return Symbols;
            }
        }

        internal class SymbolItem : ITreeNode
        {
            public PublicNameDefinition Symbol { get; private set; }

            public SymbolItem(PublicNameDefinition symbol)
            {
                if (symbol == null)
                    throw new ArgumentNullException("symbol");
                this.Symbol = symbol;
            }

            public override string ToString()
            {
                if (Symbol.BaseSegment == null)
                {
                    return string.Format("{0} : {1:X4}:{2:X4}",
                        Symbol.Name, Symbol.BaseFrame, Symbol.Offset);
                }
                else
                {
                    return string.Format("{0} : {1}+{2:X}h",
                        Symbol.Name, Symbol.BaseSegment.SegmentName, Symbol.Offset);
                }
            }

            public string Text
            {
                get { return this.ToString(); }
            }

            public bool HasChildren
            {
                get { return false; }
            }

            public IEnumerable<ITreeNode> GetChildren()
            {
                return null;
            }
        }
    }
}