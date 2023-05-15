namespace VotingApp.Base.Domain
{
    public interface IBaseService<T> where T : Entity
    {
        public IResponse Delete(DeleteDTO deleteDTO);

        public IResponse GetById(GetByIDDTO getByIDDTO);
       
    }
}
