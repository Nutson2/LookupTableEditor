using CommunityToolkit.Mvvm.ComponentModel;

namespace LookupTableEditor.Views
{
    public partial class TableDefinitionPageViewModel : ObservableObject
    {
        private readonly SizeTableInfo _sizeTableInfo;

        public TableDefinitionPageViewModel(SizeTableInfo sizeTableInfo)
        {
            _sizeTableInfo = sizeTableInfo;
        }
    }
}
