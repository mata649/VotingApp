namespace VotingApp.Base.Domain
{
    public class Pagination
    {
        private ushort _currentPage = 1;
        private ushort _pageSize = 10;

        public ushort CurrentPage
        {
            get => _currentPage; set
            {
                _currentPage = (ushort)(value > 0 ? value : 1);

            }
        }
        public ushort PageSize
        {
            get => _pageSize; set
            {
                _pageSize = (ushort)(value > 0 ? value : 10);
            }
        }
    }
}
