using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System;
using System.Windows;

namespace Micro.Future.ViewModel
{
    /// <summary>
    /// The ViewModel for the LoadOnDemand demo.  This simply
    /// exposes a read-only collection of regions.
    /// </summary>
    public class CountryViewModel
    {
        readonly ReadOnlyCollection<RegionViewModel> _regions;

        public CountryViewModel(string[] regions, bool byProductClassOrExchange)
        {
            _regions = new ReadOnlyCollection<RegionViewModel>(
                (from region in regions
                 select new RegionViewModel(region, byProductClassOrExchange))
                .ToList());

            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        public ReadOnlyCollection<RegionViewModel> Regions
        {
            get { return _regions; }
        }

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingNodeEnumerator = null;
            }
        }

        #endregion // SearchText
        readonly ICommand _searchCommand;

        IEnumerator<TreeViewItemViewModel> _matchingNodeEnumerator;
        string _searchText = String.Empty;

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly CountryViewModel _familyTree;

            public SearchFamilyTreeCommand(CountryViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingNodeEnumerator == null || !_matchingNodeEnumerator.MoveNext())
                this.VerifyMatchingNodeEnumerator();

            var node = _matchingNodeEnumerator.Current;

            if (node == null)
                return;

            // Ensure that this person is in view.
            if (node.Parent != null)
                node.Parent.IsExpanded = true;

            node.IsSelected = true;
        }

        void VerifyMatchingNodeEnumerator()
        {
            var matches = this.FindMatches(_searchText, _regions);
            _matchingNodeEnumerator = matches.GetEnumerator();

            if (!_matchingNodeEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<TreeViewItemViewModel> FindMatches(string searchText, IEnumerable<TreeViewItemViewModel> nodes)
        {
            if (nodes != null)
            {

                foreach (TreeViewItemViewModel node in nodes)
                {
                    if (node.NameContainsText(searchText))
                        yield return node;
                }

                foreach (TreeViewItemViewModel node in nodes)
                    foreach (TreeViewItemViewModel match in FindMatches(searchText, node.Children))
                        yield return match;
            }
        }

        #endregion // Search Logic
    }
}